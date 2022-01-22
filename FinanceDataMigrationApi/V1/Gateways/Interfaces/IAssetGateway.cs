using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAssetGateway
    {
        public Task<APIResponse<GetAllAssetListResponse>> DownloadAsync(string lastHintStr);
    }
}
