using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Hackney.Shared.HousingSearch.Domain.Asset;


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

        public async Task<AssetPaginationResponse> DownloadAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey)
        {
            var lastHintStr =
                lastEvaluatedKey == null || !lastEvaluatedKey.ContainsKey("id") || lastEvaluatedKey["id"].NULL || Guid.Parse(lastEvaluatedKey["id"].S) == Guid.Empty
                    ? ""
                    : lastEvaluatedKey["id"].S;

            var uri = new Uri($"{_client.BaseAddress}/search/assets/all?searchText=**&pageSize={count}&page=1&sortBy=id&isDesc=true&lastHitId={lastHintStr}", UriKind.Absolute);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var assetsResponse = await response.ReadContentAs<APIResponse<GetAssetListResponse>>().ConfigureAwait(true);
            return new AssetPaginationResponse()
            {
                LastKey = new Dictionary<string, AttributeValue>
                {
                    {"id",new AttributeValue{S = assetsResponse?.lastHitId}}
                },
                Assets = assetsResponse?.Results?.Assets
            };
        }

        public async Task<AssetPaginationResponse> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(AssetGateway)}" +
                                   $".{nameof(GetAll)} Scan started.");

            ScanRequest request = new ScanRequest("Assets");
            if (lastEvaluatedKey != null)
            {
                if (lastEvaluatedKey.ContainsKey("id") && lastEvaluatedKey["id"].S != Guid.Empty.ToString())
                {
                    request.ExclusiveStartKey = lastEvaluatedKey;
                }
            };

            ScanResponse response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
            if (response?.Items == null || response.Items.Count == 0)
                throw new Exception($"_dynamoDb.ScanAsync result is null");

            LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(AssetGateway)}" +
                                   $"{nameof(GetAll)} Scan finished with {response?.Items?.Count} records and Evaluated key is: {response?.LastEvaluatedKey["id"].S}.");

            return new AssetPaginationResponse()
            {
                LastKey = response?.LastEvaluatedKey,
                Assets = response?.ToAssets()?.ToList()
            };
        }

        public async Task<List<Hackney.Shared.Asset.Domain.Asset>> GetAllBySegmentScan()
        {
            int totalSegments = 5;
            var finalResult = new List<Hackney.Shared.Asset.Domain.Asset>();
            LoggingHandler.LogInfo($"*** Creating {totalSegments} Parallel Scan Tasks to scan Assets");
            Task[] tasks = new Task[totalSegments];
            for (int segment = 0; segment < totalSegments; segment++)
            {
                int tmpSegment = segment;
                Task task = await Task.Factory.StartNew(async () =>
                {
                    var scanSegmentResult = await ScanSegment(totalSegments, tmpSegment).ConfigureAwait(false);

                    finalResult.AddRange(scanSegmentResult);
                }).ConfigureAwait(false);

                tasks[segment] = task;
            }

            LoggingHandler.LogInfo("All scan tasks are created, waiting for them to complete.");
            Task.WaitAll(tasks);

            LoggingHandler.LogInfo("All scan tasks are completed.");
            LoggingHandler.LogInfo($"*** Completed Count:  {finalResult.Count} ");


            LoggingHandler.LogInfo("Scan completed");

            // return finalResult;

            return finalResult;
        }
        private async Task<List<Hackney.Shared.Asset.Domain.Asset>> ScanSegment(int totalSegments, int segment)
        {
            var resultList = new List<Hackney.Shared.Asset.Domain.Asset>();

            LoggingHandler.LogInfo($"*** Starting to Scan Segment {segment} of Assets out of {totalSegments} total segments ***");
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;
            int totalScannedItemCount = 0;
            int totalScanRequestCount = 0;
            do
            {
                var request = new ScanRequest
                {
                    TableName = "Assets",
                    Limit = 1200,
                    ExclusiveStartKey = lastEvaluatedKey,
                    Segment = segment,
                    TotalSegments = totalSegments
                };

                var response = await _dynamoDb.ScanAsync(request).ConfigureAwait(false);
                lastEvaluatedKey = response.LastEvaluatedKey;
                totalScanRequestCount++;
                totalScannedItemCount += response.ScannedCount;

                var scannedResult = response.ToAssetsDomain();
                LoggingHandler.LogInfo($"*** Completed Scan Count : {scannedResult.Count()} ");
                
                resultList.AddRange(scannedResult);
                LoggingHandler.LogInfo($"*** Completed Filtered Count:  {resultList.Count} ");
                Thread.Sleep(2000);
            } while (lastEvaluatedKey.Count != 0);

            LoggingHandler.LogInfo($"*** Completed Scan Segment {segment} of Assets. TotalScanRequestCount: {totalScanRequestCount}, TotalScannedItemCount: {totalScannedItemCount} ***");
            return resultList;
        }
        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml)
        {
            return _dbContext.InsertDynamoAsset(lastHint, xml);
        }
    }
}
