using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts
{
    public interface IDeleteAllAccountEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
