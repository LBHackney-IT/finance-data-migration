using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmRunStatusSaveUseCase : IDmRunStatusSaveUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmRunStatusSaveUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(DmRunStatusModel model)
        {
            await _gateway.SaveStatus(model).ConfigureAwait(false);
        }
    }
}
