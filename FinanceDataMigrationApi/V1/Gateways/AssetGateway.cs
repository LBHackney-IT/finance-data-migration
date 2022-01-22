using System;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway: IAssetGateway
    {
        private readonly HttpClient _client;

        public AssetGateway(HttpClient client)
        {
            _client = client;
        }
        public async Task<APIResponse<GetAllAssetListResponse>> DownloadAsync(string lastHintStr = "")
        {
            var uri = new Uri($"api/v1/search/assets/all?searchText=**&pageSize=100&page=1&{lastHintStr}sortBy=id&isDesc=true", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var assetsResponse = await response.ReadContentAs<APIResponse<GetAllAssetListResponse>>().ConfigureAwait(true);
            return assetsResponse;
        }
    }
}
