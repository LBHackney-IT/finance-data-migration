using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway : IAssetGateway
    {
        private readonly HttpClient _client;
        private readonly DatabaseContext _dbContext;

        public AssetGateway(HttpClient client, DatabaseContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _client = client;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(contextAccessor.HttpContext.Request.Headers["Authorization"]);
            _dbContext = dbContext;
        }
        public async Task<APIResponse<GetAssetListResponse>> DownloadAsync(string lastHintStr = "")
        {
            var uri = new Uri($"api/v1/search/assets/all?searchText=**&pageSize=5000&page=1&sortBy=id&isDesc=true&lastHitId={lastHintStr}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var assetsResponse = await response.ReadContentAs<APIResponse<GetAssetListResponse>>().ConfigureAwait(true);
            return assetsResponse;
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
