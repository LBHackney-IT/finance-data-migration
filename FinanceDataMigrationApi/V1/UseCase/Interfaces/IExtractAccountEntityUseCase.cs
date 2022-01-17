using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IExtractAccountEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
