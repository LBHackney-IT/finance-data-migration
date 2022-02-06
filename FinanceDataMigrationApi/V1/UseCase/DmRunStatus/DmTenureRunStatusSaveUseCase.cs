using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmTenureRunStatusSaveUseCase : IDmTenureRunStatusSaveUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmTenureRunStatusSaveUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(bool status)
        {
            var model = await _gateway.GetData().ConfigureAwait(false);
            model.AllTenureDmCompleted = status;
            await _gateway.SaveStatus(model).ConfigureAwait(false);
        }
    }
}
