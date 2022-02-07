using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
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

        public TransactionGateway(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _context = context;
        }

        public async Task<int> ExtractAsync()
        {
            return await _context.ExtractDmTransactionsAsync().ConfigureAwait(false);
        }

        public async Task<IList<DmTransaction>> GetTransformedListAsync(int count)
        {
            var results = await _context.GetTransformedTransactionListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Loading);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<DmTransaction>> GetExtractedListAsync(int count)
        {
            var results = await _context.GetExtractedTransactionListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Loading);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
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
    }
}
