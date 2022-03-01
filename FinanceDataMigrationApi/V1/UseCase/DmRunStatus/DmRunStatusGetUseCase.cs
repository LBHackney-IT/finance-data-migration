using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;

namespace FinanceDataMigrationApi.V1.UseCase.DmRunStatus
{
    public class DmRunStatusGetUseCase : IDmRunStatusGetUseCase
    {
        private readonly IDmRunStatusGateway _gateway;

        public DmRunStatusGetUseCase(IDmRunStatusGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<DmRunStatusModel> ExecuteAsync()
        {
            var data = await _gateway.GetData().ConfigureAwait(false);
            LoggingHandler.LogInfo($"{data.AccountExtractDate}/{data.AccountLoadDate}" +
                                   $"/{data.ChargeExtractDate}/{data.ChargeLoadDate}" +
                                   $"/{data.TransactionExtractDate}/{data.TransactionLoadDate}");
            return data;
        }
    }
}
