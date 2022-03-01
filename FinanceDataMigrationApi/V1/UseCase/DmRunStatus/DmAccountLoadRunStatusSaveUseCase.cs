using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmAccountLoadRunStatusSaveUseCase : IDmAccountLoadRunStatusSaveUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmAccountLoadRunStatusSaveUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task ExecuteAsync(DateTime dateTime)
        {
            var model = await _gateway.GetData().ConfigureAwait(false);
            model.AccountLoadDate = dateTime;
            await _gateway.SaveStatus(model).ConfigureAwait(false);
        }
    }
}
