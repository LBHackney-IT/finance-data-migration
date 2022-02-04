using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDmRunStatusGateway
    {

        public Task<DmRunStatusModel> GetStatus();
        public Task SaveStatus(DmRunStatusModel model);

    }
}
