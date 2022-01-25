using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using Hackney.Shared.Tenure.Domain;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway : ITenureGateway
    {
        private readonly DatabaseContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ILogger<ITenureGateway> _logger;

        public TenureGateway(DatabaseContext dbContext, IAmazonDynamoDB dynamoDb, ILogger<ITenureGateway> logger)
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

        public async Task<TenurePaginationResponse> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey=null)
        {
            ScanRequest request = new ScanRequest("TenureInformation")
            {
                Limit = 1000,
                ExclusiveStartKey = lastEvaluatedKey
            };
            ScanResponse response =await _dynamoDb.ScanAsync(request).ConfigureAwait(false);

            return new TenurePaginationResponse()
            {
                LastKey = response.LastEvaluatedKey,
                TenureInformation = response.ToTenureInformation().ToList()
            };
        }

        public Task<int> SaveTenuresIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoTenure(lastHint, xml);
        }
    }
}
