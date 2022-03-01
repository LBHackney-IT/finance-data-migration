using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetTenureByIdUseCase : IGetTenureByIdUseCase
    {
        private readonly ITenureGateway _gateway;

        public GetTenureByIdUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<TenureInformation> ExecuteAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException(nameof(id).ToString());

            return await _gateway.GetByIdAsync(id).ConfigureAwait(false);
        }
    }
}
