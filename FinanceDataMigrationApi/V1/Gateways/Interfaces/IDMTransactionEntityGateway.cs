using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMTransactionEntityGateway
    {
        Task<IList<DmTransaction>> ListAsync();

        Task<int> ExtractAsync(DateTimeOffset? processingDate);

        Task UpdateDMTransactionEntityItems(IList<DmTransaction> dMTransactionEntityDomainItems);

        Task<IList<DmTransaction>> GetTransformedListAsync();

        Task<IList<DmTransaction>> GetLoadedListAsync();

        Task<int> AddTransactionAsync(DmTransaction dmEntity);
    }
}
