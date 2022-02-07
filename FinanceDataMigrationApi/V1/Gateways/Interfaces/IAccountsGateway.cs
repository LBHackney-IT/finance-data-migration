using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain.Accounts;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAccountsGateway
    {
        Task<int> ExtractAsync();
        Task<IList<DmAccount>> GetExtractedListAsync(int count);
        Task BatchInsert(List<DmAccount> accounts);
    }
}
