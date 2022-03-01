using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class AssetGetAllUseCase : IAssetGetAllUseCase
    {
        private readonly IAssetGateway _gateway;

        public AssetGetAllUseCase(IAssetGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<APIResponse<GetAssetListResponse>> ExecuteAsync(int count, string lastHint)
        {
            /*return await _gateway.GetAll(count, lastEvaluatedKey).ConfigureAwait(false);*/
            return await _gateway.DownloadAsync(count, lastHint).ConfigureAwait(false);
        }
    }
}
