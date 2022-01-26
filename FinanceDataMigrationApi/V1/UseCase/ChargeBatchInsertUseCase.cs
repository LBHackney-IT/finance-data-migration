using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class ChargeBatchInsertUseCase: IChargeBatchInsertUseCase
    {
        private readonly IChargeGateway _gateway;

        public ChargeBatchInsertUseCase(IChargeGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<bool> ExecuteAsync(List<Charge> charges)
        {
            return await _gateway.BatchInsert(charges).ConfigureAwait(false);
        }
    }
}
