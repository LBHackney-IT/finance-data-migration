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
        public Task<APIResponse<GetAssetListResponse>> ExecuteAsync(string lastHintStr)
        {
            return _gateway.DownloadAsync(lastHintStr);
        }
    }
}
