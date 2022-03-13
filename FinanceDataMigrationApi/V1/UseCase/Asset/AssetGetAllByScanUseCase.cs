using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset;

namespace FinanceDataMigrationApi.V1.UseCase.Asset
{
    public class AssetGetAllByScanUseCase : IAssetGetAllByScanUseCase
    {
        private readonly IAssetGateway _gateway;

        public AssetGetAllByScanUseCase(IAssetGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<AssetPaginationResponse> ExecuteAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey)
        {
            return await _gateway.DownloadAsync(count, lastEvaluatedKey).ConfigureAwait(false);
            /*return await _gateway.GetAll(count, lastEvaluatedKey).ConfigureAwait(false);*/
        }
    }
}
