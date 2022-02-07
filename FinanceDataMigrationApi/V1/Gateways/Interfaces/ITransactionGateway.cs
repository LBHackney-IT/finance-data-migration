using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        public Task<int> ExtractAsync();
        public Task<IList<DmTransaction>> GetExtractedListAsync(int count);
        public Task BatchInsert(List<DmTransaction> transactions);

    }
}
