using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITimeLogGateway
    {
        public Task Save(DmTimeLogModel timeLogModel);
    }
}
