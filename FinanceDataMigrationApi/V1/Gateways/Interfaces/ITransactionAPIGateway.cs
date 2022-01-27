using Hackney.Shared.HousingSearch.Domain.Transactions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionAPIGateway
    {
        public Task<int> UpdateTransactionItems(IList<Transaction> items);

        public Task UpdateTransaction(Transaction transaction);

    }
}
