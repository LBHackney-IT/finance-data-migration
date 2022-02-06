using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmAssetRunStatusSaveUseCase
    {
        public Task ExecuteAsync(bool status);
    }
}
