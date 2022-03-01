using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmAccountLoadRunStatusSaveUseCase
    {
        public Task ExecuteAsync(DateTime dateTime);
    }
}
