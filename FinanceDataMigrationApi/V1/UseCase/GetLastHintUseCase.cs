using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetLastHintUseCase : IGetLastHintUseCase
    {
        private readonly IHitsGateway _gateway;

        public GetLastHintUseCase(IHitsGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<Guid> ExecuteAsync()
        {
            return await _gateway.GetLastHint().ConfigureAwait(false);
        }
    }
}
