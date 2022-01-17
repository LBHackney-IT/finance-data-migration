using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITransformAccountEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
        public Task<string> GetAccountAsync(string paymentReference);
    }
}
