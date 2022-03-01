using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmChargeLoadRunStatusSaveUseCase
    {
        public Task ExecuteAsync(DateTime dateTime);
    }
}
