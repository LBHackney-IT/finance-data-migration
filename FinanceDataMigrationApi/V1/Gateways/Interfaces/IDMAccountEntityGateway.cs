using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMAccountEntityGateway
    {
        Task<IList<DMAccountEntity>> GetLoadedListAsync();
    }
}
