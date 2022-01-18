using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetChargesByIdUseCase : IGetConsolidatedChargesByIdUseCase
    {
        private readonly IConsolidatedChargesApiGateway _consolidatedChargesApiGateway;
        public GetChargesByIdUseCase(IConsolidatedChargesApiGateway consolidatedChargesApiGateway)
        {
            _consolidatedChargesApiGateway = consolidatedChargesApiGateway;
        }
        public async Task<List<ConsolidatedCharge>> ExecuteAsync(Guid targetId)
        {
            return (await _consolidatedChargesApiGateway.GetConsolidatedtChargesByIdAsync(targetId).ConfigureAwait(false));
        }
    }
}
