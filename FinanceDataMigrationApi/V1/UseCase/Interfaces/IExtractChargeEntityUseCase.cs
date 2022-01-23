using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IExtractChargeEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
