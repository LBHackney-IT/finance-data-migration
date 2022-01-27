using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IChargeBatchInsertUseCase
    {
        Task<bool> ExecuteAsync(List<Charge> charges);
    }
}