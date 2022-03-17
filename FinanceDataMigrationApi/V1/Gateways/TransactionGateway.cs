using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using System.Threading.Tasks;
using AutoMapper.Internal;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionGateway : ITransactionGateway
    {
        private const string DataMigrationTask = "INDEXING";

        private readonly EsGateway<QueryableTransaction> _esGateway;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        readonly DatabaseContext _context;
        private readonly HttpClient _client;

        public TransactionGateway(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _context = context;
        }
        public TransactionGateway(HttpClient client)
        {
            _client = client;
        }

        public TransactionGateway(EsGateway<QueryableTransaction> esGateway)
        {
            _esGateway = esGateway;
        }
        public async Task<int> ExtractAsync()
        {
            return await _context.ExtractDmTransactionsAsync().ConfigureAwait(false);
        }
        public async Task<IList<DmTransaction>> GetExtractedListAsync(int count)
        {
            try
            {
                /*var accounts = await _context.AccountDbEntities.Where(ac =>
                    ac.MigrationStatus == EMigrationStatus.Loaded && ac.TargetId != null)
                    .Take(count)
                    .Select(t => t.TargetId)
                    .ToListWithNoLockAsync()
                    .ConfigureAwait(false);*/

                var results = await _context.TransactionEntities
                    .Where(x => x.MigrationStatus == EMigrationStatus.Extracted /*&& accounts.Contains(x.TargetId)*/)
                    //.OrderBy(ac1 => ac1.TargetId)
                    .Take(count)
                    .ToListWithNoLockAsync()
                    .ConfigureAwait(false);

                results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Loading);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return results.ToDomains();
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(TransactionGateway)}.{nameof(GetExtractedListAsync)} Exception: {ex.GetFullMessage()}");
                throw;
            }
        }
        public async Task BatchInsert(List<DmTransaction> transactions)
        {
            DatabaseContext context = DatabaseContext.Create();
            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (DmTransaction transaction in transactions)
            {
                var columns = transaction.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "Transactions",
                        Item = columns,
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                        ConditionExpression = "attribute_not_exists(id)"
                    }
                });
            }

            TransactWriteItemsRequest placeOrderCharge = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            try
            {
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);

                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.Loaded);
                await context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (ResourceNotFoundException rnf)
            {
                LoggingHandler.LogError($"One of the table involved in the account is not found: {rnf.Message}.");
                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogError($"Internal Server Error: {ise.Message}");
                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogError($"Transaction Canceled: {tce.Message}");
                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            finally
            {
                await context.DisposeAsync().ConfigureAwait(false);
            }
        }
        public async Task<List<DmTransaction>> GetLoadedListAsync(int count)
        {
            var results = await _context.GetLoadedTransactionListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Indexing);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomains();
        }
        public async Task<int> UpdateTransactionItems(IList<Transaction> transactions)
        {
            var response = await _client.PostAsJsonAsyncType(new Uri("api/v1/transactions/process-batch", UriKind.Relative), transactions)
                .ConfigureAwait(true);
            return response ? transactions.Count : 0;
        }
        public async Task<Task> BulkIndex(List<QueryableTransaction> transactions)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));

            DatabaseContext context = DatabaseContext.Create();
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity.");

            if (transactions.Any())
            {
                await _esGateway.BulkIndex(transactions).ConfigureAwait(false);
                context.TransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.IdDynamodb))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.Indexed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity");
            }
            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");

            return Task.CompletedTask;
        }
    }
}
