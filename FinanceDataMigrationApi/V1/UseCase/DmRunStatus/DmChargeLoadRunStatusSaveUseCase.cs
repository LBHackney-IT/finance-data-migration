using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmChargeLoadRunStatusSaveUseCase : IDmChargeLoadRunStatusSaveUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmChargeLoadRunStatusSaveUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task ExecuteAsync(DateTime dateTime)
        {
            var model = await _gateway.GetData().ConfigureAwait(false);
            model.ChargeLoadDate = dateTime;
            await _gateway.SaveStatus(model).ConfigureAwait(false);
        }
    }
}
