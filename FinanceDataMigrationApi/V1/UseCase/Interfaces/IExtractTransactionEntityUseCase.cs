using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public interface IExtractTransactionEntityUseCase 
    {
        public Task<StepResponse> ExecuteAsync();
    }
}
