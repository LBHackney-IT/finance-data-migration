using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions
{
    public interface ITransactionBatchInsertUseCase
    {
        Task ExecuteAsync(List<DmTransaction> transactions);
    }
}
