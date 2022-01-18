using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureInformationGateway : ITenureInformationGateway
    {
        public Task<TenureInformation> GetByPrnAsync(string prn)
        {
            throw new System.NotImplementedException();
        }
    }
}
