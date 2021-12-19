using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using System.Net.Http.Headers;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionGateway : ITransactionGateway
    {
        private readonly HttpClient _client;

        public TransactionGateway(HttpClient client)
        {
            _client = client;
        }
        public Task UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateTransactionItems(IList<Transaction> transactions)
        {
            var response = await _client.PostAsJsonAsyncType(new Uri("api/v1/transactions/process-batch", UriKind.Relative), transactions)
                .ConfigureAwait(true);
            return response ? transactions.Count : 0;
        }
    }
}
