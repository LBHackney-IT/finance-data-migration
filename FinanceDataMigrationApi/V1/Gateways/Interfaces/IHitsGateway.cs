using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IHitsGateway
    {
        public Task<Guid> GetLastHint();
    }
}
