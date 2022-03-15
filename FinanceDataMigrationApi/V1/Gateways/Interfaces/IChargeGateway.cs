using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IChargeGateway
    {
        Task<int> ExtractAsync();
        Task<IList<DmCharge>> GetExtractedListAsync(int count);
        Task BatchInsert(List<DmCharge> charges);
        Task RemoveTable();
    }
}
