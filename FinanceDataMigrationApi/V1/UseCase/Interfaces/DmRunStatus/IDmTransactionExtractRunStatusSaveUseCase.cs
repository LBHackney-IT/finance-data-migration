using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmTransactionExtractRunStatusSaveUseCase
    {
        public Task ExecuteAsync(DateTime dateTime);
    }
}
