using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Factories;
using Hackney.Shared.Tenure.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway : ITenureGateway
    {
        private readonly DatabaseContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<ITenureGateway> _logger;

        public TenureGateway(DatabaseContext dbContext, IAmazonDynamoDB dynamoDb, IDynamoDBContext dynamoDbContext, ILogger<ITenureGateway> logger)
        {
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
            _dynamoDbContext = dynamoDbContext;
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

        public async Task<TenureInformation> GetByIdAsync(Guid id)
        {
            var result = await _dynamoDbContext.LoadAsync<TenureInformationDb>(id).ConfigureAwait(false);
            return result.ToDomain();
        }

        public async Task<bool> BatchInsert(List<TenureInformation> tenures)
        {
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

            await _dynamoDb.TransactWriteItemsAsync(placeOrderTransaction).ConfigureAwait(false);
            return true;
        }

        public async Task<TenurePaginationResponse> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            string step = "GetAll";
            try
            {
                ScanRequest request = new ScanRequest("TenureInformation")
                {
                    Limit = 1000,
                    ExclusiveStartKey = lastEvaluatedKey
                };
                step = "GetAll+ScanRequest";
                ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
                step = "GetAll+_dynamoDb.ScanAsync";
                if (response == null || response.Items == null)
                    throw new Exception($"_dynamoDb.ScanAsync results NULL: {response?.ToString()}");

                return new TenurePaginationResponse()
                {
                    LastKey = response?.LastEvaluatedKey,
                    TenureInformation = response?.ToTenureInformation().ToList()
                };
            }
            catch (Exception exception)
            {
                throw new Exception($"{exception}:step{step}");
            }
        }

        public Task<int> SaveTenuresIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoTenure(lastHint, xml);
        }

        public async Task<Guid> GetLastHint()
        {
            var result = await _dbContext.DmDynamoLastHInt
                .Where(p => p.TableName.ToLower() == "tenure")
                .OrderBy(p => p.Timex).LastOrDefaultAsync().ConfigureAwait(false);
            return result?.Id ?? Guid.Empty;
        }
    }
}
