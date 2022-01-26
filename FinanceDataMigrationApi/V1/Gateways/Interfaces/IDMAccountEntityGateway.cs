using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMAccountEntityGateway
    {
        Task<IList<DMAccountEntity>> GetLoadedListAsync();

        Task<int> ExtractAsync(DateTimeOffset? processingDate);

        Task<IList<DMAccountEntity>> ListAsync();

        Task<IList<DMAccountEntity>> GetTransformedListAsync();

        Task UpdateDMAccountEntityItems(IList<DMAccountEntity> dMAccountEntities);

        Task SaveTenureListAsync(IList<DMTenureEntity> tenureEntities);
    }
}
