using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureAPIGateway
    {
        public Task<List<TenureInformation>> GetByPrnAsync(string prn);
    }
}
