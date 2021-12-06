using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DynamoDbGateway : IMigrationRunDynamoGateway
    {
        //private const string MigrationRunDynamoDbTableName = "MigrationRuns";
        private const string DynamodbEntity = "dynamodb_entity";
        private const string IsFeatureEnabled = "is_feature_enabled";

        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        //private readonly IEntityUpdater _updater;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb)
        {
            _dynamoDbContext = dynamoDbContext;
            _amazonDynamoDb = amazonDynamoDb;
        }

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task AddAsync(MigrationRun migrationRun)
        {
            await _dynamoDbContext.SaveAsync(migrationRun.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<MigrationRun> GetMigrationRunByIdAsync(Guid id)
        {
            var result = await _dynamoDbContext.LoadAsync<MigrationRunDbEntity>(id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public async Task<MigrationRun> GetMigrationRunByEntityNameAsync(string entityName)
        {
            var dbMigrationRuns = new List<MigrationRunDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<MigrationRunDbEntity>();

            //var expressionAttributeValues = new Dictionary<string, DynamoDBEntry>
            //{
            //    { ":V_dynamodb_entity", entityName },
            //    //{ ":V_is_feature_enabled", true }
            //};

            var queryOperationConfig = new QueryOperationConfig()
            {
                //KeyExpression = new Expression
                //{
                //    ExpressionStatement = expressionAttributeValues
                //},
                //KeyExpression = new Expression
                //{
                //    ExpressionStatement = "dynamodb_entity = :V_dynamodb_entity",
                //    ExpressionAttributeValues = expressionAttributeValues
                //},
                Filter = new QueryFilter(DynamodbEntity, QueryOperator.Equal, entityName)
            };
            queryOperationConfig.Filter.AddCondition(IsFeatureEnabled, QueryOperator.Equal, true);

            var search = table.Query(queryOperationConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            if (resultsSet.Any())
            {
                dbMigrationRuns.AddRange(_dynamoDbContext.FromDocuments<MigrationRunDbEntity>(resultsSet));
            }
            //return new MigrationRun(dbMigrationRuns.Select(x => x.ToDomain()), new PaginationDetails(paginationToken));

            return (MigrationRun) dbMigrationRuns.Select(x => x.ToDomain());
        }

        public async Task UpdateAsync(MigrationRun migrationRun)
        {
            migrationRun.UpdatedAt = DateTime.Now;
            await _dynamoDbContext.SaveAsync(migrationRun.ToDatabase()).ConfigureAwait(false);
        }


        //////public async Task<MigrationRunResponseList> GetAllMigrationRunsAsync()
        public async Task<List<MigrationRun>> GetAllMigrationRunsAsync()
        {
            var conditions = new List<ScanCondition>();
            var dbMigrationRuns = await _dynamoDbContext.ScanAsync<MigrationRunDbEntity>(conditions).GetRemainingAsync().ConfigureAwait(false);
            return dbMigrationRuns.ToDomain();
        }


        //public async Task<UpdateEntityResult<MigrationRunDbEntity>> UpdateAsync(MigrationRunUpdateRequest requestObject, string requestBody, Guid id)
        //{
        //    var existingMigrationRun = await _dynamoDbContext.LoadAsync<MigrationRunDbEntity>(id).ConfigureAwait(false);
        //    if (existingMigrationRun == null) return null;

        //    var result = _updater.UpdateEntity(existingMigrationRun, requestBody, requestObject);
        //    if (result.NewValues.Any())
        //    {
        //        result.UpdatedEntity.UpdatedAt = DateTime.UtcNow;
        //        await _dynamoDbContext.SaveAsync(result.UpdatedEntity).ConfigureAwait(false);
        //    }

        //    return result;
        //}


        ////public async Task<MigrationRun> GetMigrationRunByEntityNameAsync(string entityName)
        ////{
        ////    //var result = await _dynamoDbContext.LoadAsync<MigrationRunDbEntity>(entityName).ConfigureAwait(false);
        ////    //return result?.ToDomain();

        ////    QueryRequest request = new QueryRequest
        ////    {
        ////        TableName = MigrationRunDynamoDbTableName,
        ////        IndexName = "dynamodb_entity_dx",
        ////        KeyConditionExpression = "dynamodb_entity = :V_dynamodb_entity",
        ////        FilterExpression = "is_feature_enabled = :V_is_feature_enabled",
        ////        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        ////        {
        ////            {":V_dynamodb_entity",new AttributeValue{S = entityName }},
        ////            {":V_is_feature_enabled",new AttributeValue{BOOL = true }}
        ////        },
        ////        ScanIndexForward = true
        ////    };

        ////    var response = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

        ////    //List<MigrationRun> data = response.ToMigrationRun();

        ////    //return data.Sort(sortBy, direction).ToList();
        ////    //return data;

        ////    //return data.ToList();
        ////    return new MigrationRun(); ;

        ////}

    }
}
