using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmTenureRunStatusSaveUseCase
    {
        public Task ExecuteAsync(bool status);
    }
}
