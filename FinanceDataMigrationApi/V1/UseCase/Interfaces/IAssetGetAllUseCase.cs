using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IAssetGetAllUseCase
    {
        public Task<APIResponse<GetAssetListResponse>> ExecuteAsync(string lastHintStr);
    }
}
