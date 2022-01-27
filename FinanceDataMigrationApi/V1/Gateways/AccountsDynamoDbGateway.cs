using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AccountsDynamoDbGateway : IAccountsDynamoDbGateway
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<AccountsDynamoDbGateway> _logger;

        public AccountsDynamoDbGateway(IAmazonDynamoDB amazonDynamoDb, ILogger<AccountsDynamoDbGateway> logger)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _logger = logger;
        }

        public async Task<bool> BatchInsert(List<DMAccountEntity> accounts)
        {
            bool result = false;

            if (accounts == null || !accounts.Any())
            {
                _logger.LogInformation("There is no accounts to save in DynamoDm");
                return true;
            }

            List<TransactWriteItem> actions = new List<TransactWriteItem>(accounts.Count);

            foreach (var account in accounts)
            {
                Dictionary<string, AttributeValue> columns = account.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "Accounts",
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
                _logger.LogError($"One of the table involved in the account is not found: {rnf.Message}");
            }
            catch (InternalServerErrorException ise)
            {
                _logger.LogError($"Internal Server Error: {ise.Message}");
            }
            catch (TransactionCanceledException tce)
            {
                _logger.LogError($"Transaction Canceled: {tce.Message}");
            }

            return result;
        }
    }
}
