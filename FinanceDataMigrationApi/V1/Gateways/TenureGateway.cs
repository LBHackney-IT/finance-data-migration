using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway : ITenureGateway
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ILogger<ITenureGateway> _logger;

        public TenureGateway(IDynamoDBContext dbContext, IAmazonDynamoDB dynamoDb, ILogger<ITenureGateway> logger)
        {
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
            _logger = logger;
        }

        public async Task<List<TenureInformation>> GetByPrnAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            return await Task.Run(() => new List<TenureInformation>(5)).ConfigureAwait(false);
        }

        public Task<List<TenureInformation>> GetByPrnAsync(List<string> prn)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BatchInsert(List<TenureInformation> tenures)
        {
            bool result = false;
            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (TenureInformation tenure in tenures)
            {
                Dictionary<string, AttributeValue> columns = tenure.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "TenureInformation",
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
                await _dynamoDb.TransactWriteItemsAsync(placeOrderTransaction).ConfigureAwait(false);
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

        public async Task<TenurePaginationResponse> GetAll(string paginationToken = "{}")
        {
            /*#region Query Execution

            QueryRequest queryRequest = new QueryRequest
            {
                Limit = 1000,
                ExclusiveStartKey = lastKeyEvaluated,
                TableName = "TenureInformation",
                //IndexName = "id",
                KeyConditionExpression = "id > :V_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_id", new AttributeValue {S = Guid.Empty.ToString()}}
                }
            };

            var result = await _dynamoDb.QueryAsync(queryRequest).ConfigureAwait(false);
            lastKeyEvaluated = result.LastEvaluatedKey;

            #endregion

            return new TenurePaginationResponse
            {
                LastKey = result.LastEvaluatedKey,
                TenureInformations = result.ToTenureInformations().ToList()
            };*/

            var dbTransactions = new List<TenureInformationDb>();
            
            var table = _dbContext.GetTargetTable<TenureInformationDb>();

            var queryConfig = new QueryOperationConfig
            {
                BackwardSearch = true,
                ConsistentRead = true,
                Filter = new QueryFilter("id", QueryOperator.GreaterThan, Guid.Empty),
                PaginationToken = paginationToken
            };

            do
            {
                var search = table.Query(queryConfig);
                paginationToken = search.PaginationToken;
                //_logger.LogDebug($"Querying {queryConfig.IndexName} index for targetId {query.TargetId}");
                var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);
                if (resultsSet.Any())
                {
                    dbTransactions.AddRange(_dbContext.FromDocuments<TenureInformationDb>(resultsSet));

                }
            }
            while (!string.Equals(paginationToken, "{}", StringComparison.Ordinal));

            /*return tenureResponse?.Results?.Tenures;*/
            return new TenurePaginationResponse()
            {
                TenureInformations = dbTransactions,
                PaginationToken = paginationToken
            };
        }
    }
}
