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
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;


namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway : IAssetGateway
    {
        private readonly HttpClient _client;
        private readonly DatabaseContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;


        public AssetGateway(DatabaseContext dbContext, IAmazonDynamoDB dynamoDb)
        {
            var searchApiUrl = Environment.GetEnvironmentVariable("SEARCH_API_URL") ??
                               throw new Exception("Housing search api url is null.");
            var searchApiToken = Environment.GetEnvironmentVariable("SEARCH_API_TOKEN") ??
                                  throw new Exception("Housing search api token is null.");

            _client = new HttpClient();
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
            _client.BaseAddress = new Uri(searchApiUrl);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", searchApiToken);
        }

        public async Task<APIResponse<GetAssetListResponse>> DownloadAsync(int count, string lastHintStr = "")
        {
            var uri = new Uri($"{_client.BaseAddress}/search/assets/all?searchText=**&pageSize={count}&page=1&sortBy=id&isDesc=true&lastHitId={lastHintStr}", UriKind.Absolute);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            LoggingHandler.LogInfo($"Response:{response}");
            var assetsResponse = await response.ReadContentAs<APIResponse<GetAssetListResponse>>().ConfigureAwait(true);
            return assetsResponse;
        }

        public async Task<AssetPaginationResponse> GetAll(int count, Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            ScanRequest request = new ScanRequest("Assets")
            {
                Limit = count,
                ExclusiveStartKey = lastEvaluatedKey
            };

            ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
            if (response?.Items == null || response.Items.Count == 0)
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
