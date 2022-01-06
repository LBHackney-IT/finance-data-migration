using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMChargeEntityGateway
    {

        Task<IList<DMChargeEntityDomain>> ListAsync();

        Task <int> ExtractAsync(DateTimeOffset? processingDate);

        Task UpdateDMChargeEntityItems(IList<DMChargeEntityDomain> dMChargeEntityDomainItems);

        Task<IList<DMChargeEntityDomain>> GetTransformedListAsync();

        Task<IList<DMChargeEntityDomain>> GetLoadedListAsync();

        Task<int> AddChargeAsync(DMChargeEntityDomain dmEntity);
    }
}
