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
        public async Task<Guid> ExecuteAsync(string tableName)
        {
            return await _gateway.GetLastHint(tableName).ConfigureAwait(false);
        }
    }
}
