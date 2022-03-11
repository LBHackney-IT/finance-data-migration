using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset
{
    public interface IAssetGetAllByElasticSearchUseCase
    {
        public Task<APIResponse<GetAssetListResponse>> ExecuteAsync(int count, string lastHint);
    }
}
