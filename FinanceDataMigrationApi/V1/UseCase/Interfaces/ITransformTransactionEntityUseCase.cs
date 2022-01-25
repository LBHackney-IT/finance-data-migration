using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public interface ITransformTransactionEntityUseCase
    {
        public Task<StepResponse> ExecuteAsync();
        public Task<string> GetTransactionPersonAsync(string paymentReference);
    }
}
