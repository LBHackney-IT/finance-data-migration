using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        public Task<int> ExtractAsync();
        public Task<IList<DmTransaction>> GetExtractedListAsync(int count);
        public Task BatchInsert(List<DmTransaction> transactions);
        /*public Task BatchDelete(List<DmTransaction> transactions);
        public Task<List<DmTransaction>> GetToBeDeletedListForDeleteAsync(int count);*/
        public Task<List<DmTransaction>> GetLoadedListAsync(int count);
        public Task<int> UpdateTransactionItems(IList<Transaction> transactions);
    }
}
