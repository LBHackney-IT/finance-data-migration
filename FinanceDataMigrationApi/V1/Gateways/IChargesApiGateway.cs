using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public interface IChargesApiGateway
    {
        public Task<List<Charge>> GetChargesByIdAsync(Guid targetId);
    }
}
