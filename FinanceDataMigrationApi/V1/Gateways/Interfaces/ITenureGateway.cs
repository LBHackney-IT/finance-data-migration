using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureGateway
    {
        public Task<List<TenureInformation>> GetByPrnAsync(string prn);
    }
}
