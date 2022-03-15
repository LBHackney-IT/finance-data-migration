using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges;

namespace FinanceDataMigrationApi.V1.UseCase.Charges
{
    public class RemoveChargeTableUseCase : IRemoveChargeTableUseCase
    {
        private readonly ChargeGateway _gateway;

        public RemoveChargeTableUseCase(ChargeGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task ExecuteAsync()
        {
            await _gateway.RemoveTable().ConfigureAwait(false);
        }
    }
}
