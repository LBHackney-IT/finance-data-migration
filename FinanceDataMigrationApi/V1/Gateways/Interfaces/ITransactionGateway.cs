using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    interface ITransactionGateway
    {
        public Task<int> UpdateTransactionItems(IList<Transaction> items);

        public Task UpdateTransaction(Transaction transaction);

    }
}
