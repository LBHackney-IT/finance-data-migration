using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IChargeGateway
    {
        Task<IList<DMChargeEntityDomain>> ListAsync();

        Task<int> ExtractAsync(DateTimeOffset? processingDate);

        Task<List<DMDetailedChargesEntity>> GetDetailChargesListAsync(string paymentReference);

        Task UpdateDMChargeEntityItems(IList<DMChargeEntityDomain> dMChargeEntityDomainItems);

        Task<IList<DMChargeEntityDomain>> GetTransformedListAsync();

        Task<IList<DMChargeEntityDomain>> GetLoadedListAsync();

        Task<int> AddChargeAsync(DMChargeEntityDomain dmEntity);

        Task<bool> BatchInsert(List<Charge> charges);
    }
}
