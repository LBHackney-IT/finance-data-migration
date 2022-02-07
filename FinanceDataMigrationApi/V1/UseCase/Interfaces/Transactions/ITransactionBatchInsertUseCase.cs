using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions
{
    public interface ITransactionBatchInsertUseCase
    {
        Task<bool> ExecuteAsync(List<Transaction> transactions);
    }
}
