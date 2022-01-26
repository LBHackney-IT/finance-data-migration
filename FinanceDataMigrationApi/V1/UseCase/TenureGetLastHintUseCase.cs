using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class TenureGetLastHintUseCase : ITenureGetLastHintUseCase
    {
        private readonly ITenureGateway _gateway;

        public TenureGetLastHintUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<Guid> ExecuteAsync()
        {
            return await _gateway.GetLastHint().ConfigureAwait(false);
        }
    }
}
