using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions
{
    public interface ILoadTransactionEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync(int count);
    }
}
