using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;


namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway : IAssetGateway
    {
        private readonly DatabaseContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;

        public AssetGateway(DatabaseContext dbContext, IAmazonDynamoDB dynamoDb)
        {
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
        }

        public async Task<AssetPaginationResponse> GetAll(int count, Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            ScanRequest request = new ScanRequest("Assets")
            {
                Limit = count,
                ExclusiveStartKey = lastEvaluatedKey
            };

            ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
            if (response == null || response.Items == null || response.Items.Count == 0)
                throw new Exception($"_dynamoDb.ScanAsync results NULL: {response?.ToString()}");

            return new AssetPaginationResponse()
            {
                LastKey = response?.LastEvaluatedKey,
                Assets = response?.ToAssets()?.ToList()
            };
        }

        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoAsset(lastHint, xml);
        }
    }
}
