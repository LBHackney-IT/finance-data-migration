using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        public Task<int> UpdateTransactionItems(IList<AddTransactionRequest> items);

        public Task UpdateTransaction(Transaction transaction);

    }
}
