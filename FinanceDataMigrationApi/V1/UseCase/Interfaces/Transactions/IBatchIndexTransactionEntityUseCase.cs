using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions
{
    public interface IBatchIndexTransactionEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync(int count);
    }
}
