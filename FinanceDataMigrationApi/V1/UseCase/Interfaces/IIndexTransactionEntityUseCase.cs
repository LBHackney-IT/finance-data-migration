using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IIndexTransactionEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
