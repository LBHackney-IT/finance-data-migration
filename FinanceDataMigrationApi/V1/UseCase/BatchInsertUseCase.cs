using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class BatchInsertUseCase: IBatchInsertUseCase
    {
        private readonly ITransactionAPIGateway _gateway;

        public BatchInsertUseCase(ITransactionAPIGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<bool> ExecuteAsync(List<Transaction> transactions)
        {
            return await _gateway.BatchInsert(transactions).ConfigureAwait(false);
        }
    }
}
