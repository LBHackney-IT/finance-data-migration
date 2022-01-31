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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway : IAssetGateway
    {
        private readonly HttpClient _client;
        private readonly DatabaseContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;

        public AssetGateway(HttpClient client, DatabaseContext dbContext, IHttpContextAccessor contextAccessor, IAmazonDynamoDB dynamoDb)
        {
            _client = client;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(contextAccessor.HttpContext.Request.Headers["Authorization"]);
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
        }
        public async Task<APIResponse<GetAssetListResponse>> DownloadAsync(string lastHintStr = "")
        {
            var uri = new Uri($"api/v1/search/assets/all?searchText=**&pageSize=5000&page=1&sortBy=id&isDesc=true&lastHitId={lastHintStr}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var assetsResponse = await response.ReadContentAs<APIResponse<GetAssetListResponse>>().ConfigureAwait(true);
            return assetsResponse;
        }

        public async Task<AssetPaginationResponse> GetAll(int count, Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            try
            {
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: assetGateway");
                ScanRequest request = new ScanRequest("Assets")
                {
                    Limit = count,
                    ExclusiveStartKey = lastEvaluatedKey
                };
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: assetGateway starts scan");
                ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
                if (response == null || response.Items == null || response.Items.Count == 0)
                    throw new Exception($"_dynamoDb.ScanAsync results NULL: {response?.ToString()}");

                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: assetGateway fills response");
                return new AssetPaginationResponse()
                {
                    LastKey = response?.LastEvaluatedKey,
                    Assets = new List<Hackney.Shared.HousingSearch.Domain.Asset.Asset>() /*response?.ToAssets()?.ToList()*/
                };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(GetAll)}: Exception: {ex.Message}");
                LoggingHandler.LogError(ex.StackTrace);
                throw;
            }
        }

        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoAsset(lastHint, xml);
        }

        public async Task<Guid> GetLastHint()
        {
            var result = await _dbContext.DmDynamoLastHInt
                .Where(p => p.TableName.ToLower() == "asset")
                .OrderBy(p => p.Timex).LastOrDefaultAsync().ConfigureAwait(false);
            return result?.Id ?? Guid.Empty;
        }
    }
}
