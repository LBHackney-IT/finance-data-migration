using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmTransactionExtractRunStatusSaveUseCase: IDmTransactionExtractRunStatusSaveUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmTransactionExtractRunStatusSaveUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task ExecuteAsync(DateTime dateTime)
        {
            var model = await _gateway.GetData().ConfigureAwait(false);
            model.TransactionExtractDate = dateTime;
            await _gateway.SaveStatus(model).ConfigureAwait(false);
        }
    }
}
