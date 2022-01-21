using Hackney.Shared.Tenure.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureGateway
    {
        public Task<List<TenureInformation>> GetByPrnAsync(string prn);

        Task<List<TenureInformation>> GetTenuresByPrnAsync(List<string> prnList);
    }
}
