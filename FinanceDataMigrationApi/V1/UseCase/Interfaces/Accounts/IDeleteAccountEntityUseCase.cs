using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts
{
    public interface IDeleteAccountEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync(int count);
    }
}
