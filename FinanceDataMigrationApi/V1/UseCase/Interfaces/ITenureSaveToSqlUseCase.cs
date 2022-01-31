using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITenureSaveToSqlUseCase
    {
        public Task<int> ExecuteAsync(TenurePaginationResponse tenurePaginationResponse);
    }
}
