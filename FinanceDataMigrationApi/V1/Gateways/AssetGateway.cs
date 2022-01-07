using System;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Asset;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AssetGateway: IAssetGateway
    {
        private readonly HttpClient _client;

        public AssetGateway(HttpClient client)
        {
            _client = client;
        }

        public async Task<Asset> GetById(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw  new ArgumentNullException(nameof(id));


            var uri = new Uri($"assets/assetId/{id.TrimEnd()}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var assetResponse = await response.ReadContentAs<Asset>().ConfigureAwait(true);

            return assetResponse;
        }
    }
}
