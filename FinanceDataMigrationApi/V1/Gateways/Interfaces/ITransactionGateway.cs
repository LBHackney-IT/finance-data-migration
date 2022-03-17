using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        public Task<int> ExtractAsync();
        public Task<IList<DmTransaction>> GetExtractedListAsync(int count);
        public Task BatchInsert(List<DmTransaction> transactions);
        public Task<List<DmTransaction>> GetLoadedListAsync(int count);
        public Task<int> UpdateTransactionItems(IList<Transaction> transactions);
        public Task<Task> BulkIndex(List<QueryableTransaction> transactions);
    }
}
