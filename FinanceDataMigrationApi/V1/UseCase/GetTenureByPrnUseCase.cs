using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetTenureByPrnUseCase: IGetTenureByPrnUseCase
    {
        private readonly ITenureAPIGateway _gateway;

        public GetTenureByPrnUseCase(ITenureAPIGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<List<TenureInformation>> ExecuteAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            return await _gateway.GetByPrnAsync(prn).ConfigureAwait(false);
        }
    }
}
