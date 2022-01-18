using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionAPIGateway : ITransactionAPIGateway
    {
        private readonly HttpClient _client;

        public TransactionAPIGateway(HttpClient client)
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
