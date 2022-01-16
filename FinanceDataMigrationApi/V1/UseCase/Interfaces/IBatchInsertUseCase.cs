using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IBatchInsertUseCase
    {
        Task<bool> ExecuteAsync(List<Transaction> transactions);
    }
}
