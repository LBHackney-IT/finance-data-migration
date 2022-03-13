using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Tenure;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Tenure
{
    public class TenureBatchInsertUseCase : ITenureBatchInsertUseCase
    {
        private readonly ITenureGateway _gateway;

        public TenureBatchInsertUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<bool> ExecuteAsync(List<TenureInformation> tenures)
        {
            return await _gateway.BatchInsert(tenures).ConfigureAwait(false);
        }
    }
}
