using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class TransactionBatchInsertUseCase : ITransactionBatchInsertUseCase
    {
        private readonly ITransactionGateway _gateway;

        public TransactionBatchInsertUseCase(ITransactionGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<bool> ExecuteAsync(List<Transaction> transactions)
        {
            return await _gateway.BatchInsert(transactions).ConfigureAwait(false);
        }
    }
}
