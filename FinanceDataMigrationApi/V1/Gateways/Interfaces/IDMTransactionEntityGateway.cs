using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMTransactionEntityGateway
    {
        Task<DMTransactionEntityDomain> GetDMTransactionEntityByIdAsync(int id);

        Task<IList<DMTransactionEntityDomain>> ListAsync();

        Task <int> ExtractAsync(DateTimeOffset? processingDate);

        Task UpdateDMTransactionEntityItems(IList<DMTransactionEntityDomain> dMTransactionEntityDomainItems);

        Task<IList<DMTransactionEntityDomain>> GetTransformedListAsync();

        Task<int> AddTransactionAsync(DMTransactionEntityDomain dmEntity);
    }
}
