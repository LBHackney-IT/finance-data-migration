using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionAPIGateway
    {
        public Task<int> UpdateTransactionItems(IList<Transaction> items);

        public Task UpdateTransaction(Transaction transaction);

    }
}
