using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionGateway : ITransactionGateway
    {
        public Task UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateTransactionItems(IList<Transaction> items)
        {
            throw new NotImplementedException();
        }
    }
}
