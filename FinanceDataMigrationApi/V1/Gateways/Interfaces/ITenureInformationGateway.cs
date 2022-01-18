using FinanceDataMigrationApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureInformationGateway
    {
        public Task<TenureInformation> GetByPrnAsync(string prn);
    }
}
