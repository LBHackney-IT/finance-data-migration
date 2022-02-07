using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using System.Threading.Tasks;
using AutoMapper.Internal;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionGateway : ITransactionGateway
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly DatabaseContext _context;

        public TransactionGateway(DatabaseContext context,IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _context = context;
        }

        public async Task<IList<DmTransaction>> GetTransformedListAsync()
        {
            var results = await _context.GetTransformedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<DmTransaction>> GetLoadedListAsync()
        {
            var results = await _context.GetLoadedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            throw new NotImplementedException();
        }

        public Task<IList<DmTransaction>> GetTransformedListAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<IList<DmTransaction>> GetExtractedListAsync(int count)
        {
            throw new NotImplementedException();
        }

        public async Task BatchInsert(List<DmTransaction> transactions)
        {
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

                _context.DmTransactionEntities.Where(p =>
                    transactions.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.Loaded);
                await _context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (ResourceNotFoundException rnf)
            {
                LoggingHandler.LogError($"One of the table involved in the account is not found: {rnf.Message}");
                _context.DmTransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogError($"Internal Server Error: {ise.Message}");
                _context.DmTransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogError($"Transaction Canceled: {tce.Message}");
                _context.DmTransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                _context.DmTransactionEntities.Where(p =>
                        transactions.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }

        }

        public Task<int> UpdateTransactionItems(IList<Transaction> transactions)
        {
            /*var response = await _client.PostAsJsonAsyncType(new Uri("api/v1/transactions/process-batch", UriKind.Relative), transactions)
                .ConfigureAwait(true);
            return response ? transactions.Count : 0;*/
            throw new NotImplementedException("replaced by transaction batch process");
        }
    }
}
