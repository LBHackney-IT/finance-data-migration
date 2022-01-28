using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionGateway : ITransactionGateway
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<ITransactionGateway> _logger;

        public TransactionGateway(IAmazonDynamoDB amazonDynamoDb, ILogger<ITransactionGateway> logger)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _logger = logger;
        }
        public Task UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BatchInsert(List<Transaction> transactions)
        {
            bool result = false;
            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (Transaction transaction in transactions)
            {
                Dictionary<string, AttributeValue> columns = new Dictionary<string, AttributeValue>();
                columns = transaction.ToQueryRequest();

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

            TransactWriteItemsRequest placeOrderTransaction = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            try
            {
                await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderTransaction).ConfigureAwait(false);
                result = true;
            }
            catch (ResourceNotFoundException rnf)
            {
                _logger.LogDebug($"One of the table involved in the transaction is not found: {rnf.Message}");
            }
            catch (InternalServerErrorException ise)
            {
                _logger.LogDebug($"Internal Server Error: {ise.Message}");
            }
            catch (TransactionCanceledException tce)
            {
                _logger.LogDebug($"Transaction Canceled: {tce.Message}");
            }

            return result;
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
