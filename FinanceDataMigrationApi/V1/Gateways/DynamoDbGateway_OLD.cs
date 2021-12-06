using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DynamoDbGateway : IMigrationRunGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        //    private readonly ILogger<DynamoDbGateway> _logger;
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        //    public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger)
        //    {
        //        _dynamoDbContext = dynamoDbContext;
        //        _logger = logger;
        //    }

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb)
        {
            _dynamoDbContext = dynamoDbContext;
            _amazonDynamoDb = amazonDynamoDb;
        }

        //    [LogCall]
        public async Task<MigrationRun> GetMigrationRunByIdAsync(Guid id)
        {
            //        _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id parameter {id}");
            var result = await _dynamoDbContext.LoadAsync<MigrationRunDbEntity>(id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        public async Task<MigrationRun> GetMigrationRunByEntityNameAsync(string entityName)
        {
            var result = await _dynamoDbContext.LoadAsync<MigrationRunDbEntity>(entityName).ConfigureAwait(false);
            return result?.ToDomain();
        }

        //public async Task<MigrationRunResponseList> GetAllMigrationRunsAsync()
        public async Task<List<MigrationRun>> GetAllMigrationRunsAsync()
        {
            //    var conditions = new List<ScanCondition>();
            //    var allDocs = await _amazonDynamoDb.ScanAsync<MigrationRunResponseList>(conditions).GetRemainingAsync();
            //    return result?.ToDomain();


            return await Task.FromResult(new List<MigrationRun>()).ConfigureAwait(false);

            //MigrationRunResponseList migrationRunResponseList = new MigrationRunResponseList();
            //List<MigrationRunResponse> data = await _dynamoDbContext.LoadAsync().ConfigureAwait(false);
            //migrationRunResponseList.MigrationRunResponses = data?.Select(p => p.ToResponse()).ToList();
            //return migrationRunResponseList;
        }

        public async Task UpdateAsync(MigrationRun migrationRun)
        {
            await _dynamoDbContext.SaveAsync(migrationRun.ToDatabase()).ConfigureAwait(false);
        }
    }
}
