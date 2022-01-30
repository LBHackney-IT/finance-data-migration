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
using FinanceDataMigrationApi.V1.Handlers;
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
        readonly DatabaseContext _dbContext;
        readonly IAmazonDynamoDB _dynamoDb;
        readonly IDynamoDBContext _dynamoDbContext;
        readonly ILogger<ITenureGateway> _logger;

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

        public async Task<TenurePaginationResponse> GetAll(int count, Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            try
            {
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: tenureGateway");
                ScanRequest request = new ScanRequest("TenureInformation")
                {
                    Limit = count,
                    ExclusiveStartKey = lastEvaluatedKey
                };
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: tenureGateway starts scan");
                ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
                if (response == null || response.Items == null || response.Items.Count == 0)
                    throw new Exception($"_dynamoDb.ScanAsync results NULL: {response?.ToString()}");

                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: tenureGateway fills response");
                return new TenurePaginationResponse()
                {
                    LastKey = response?.LastEvaluatedKey,
                    TenureInformation = response?.ToTenureInformation()?.ToList()
                };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: Exception: {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }

        public Task<int> SaveTenuresIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoTenure(lastHint, xml);
        }

        public async Task<Guid> GetLastHint()
        {
            try
            {
                var result = await _dbContext.DmDynamoLastHInt.
                    Where(p => p.TableName.ToLower() == "tenure").
                    OrderBy(p => p.Timex).LastOrDefaultAsync().ConfigureAwait(false);

                return result?.Id ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetLastHint)}: Exception: {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}
