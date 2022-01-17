using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetChargesByIdUseCase : IGetChargesByIdUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;
        public GetChargesByIdUseCase(IChargesApiGateway chargesApiGateway)
        {
            _chargesApiGateway = chargesApiGateway;
        }
        public async Task<List<ChargeResponse>> ExecuteAsync(Guid targetId)
        {
            return (await _chargesApiGateway.GetChargesByIdAsync(targetId).ConfigureAwait(false)).ToResponse();
        }
    }
}
