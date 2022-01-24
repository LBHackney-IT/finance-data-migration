using Hackney.Shared.HousingSearch.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public interface IConsolidatedChargesApiGateway
    {
        public Task<List<ConsolidatedCharge>> GetConsolidatedtChargesByIdAsync(Guid targetId);
    }
}
