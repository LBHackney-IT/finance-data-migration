using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetTenureInformationByPrnUseCase : IGetTenureInformationByPrnUseCase
    {
        private readonly ITenureInformationGateway _gateway;

        public GetTenureInformationByPrnUseCase(ITenureInformationGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<TenureInformation> ExecuteAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            return await _gateway.GetByPrnAsync(prn).ConfigureAwait(false);
        }
    }
}
