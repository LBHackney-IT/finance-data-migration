using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public interface ILoadTransactionEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
