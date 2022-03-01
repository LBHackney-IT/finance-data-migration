using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmTransactionLoadRunStatusSaveUseCase
    {
        public Task ExecuteAsync(DateTime dateTime);
    }
}
