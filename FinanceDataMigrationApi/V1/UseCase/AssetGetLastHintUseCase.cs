using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class AssetGetLastHintUseCase : IAssetGetLastHintUseCase
    {
        private readonly IAssetGateway _gateway;

        public AssetGetLastHintUseCase(IAssetGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<Guid> ExecuteAsync()
        {
            return await _gateway.GetLastHint().ConfigureAwait(false);
        }
    }
}
