using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        public Task<int> UpdateTransactionItems(IList<Transaction> items);
        public Task UpdateTransaction(Transaction transaction);
        Task<bool> BatchInsert(List<Transaction> transactions);

    }
}
