using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges
{
    public interface IChargeBatchInsertUseCase
    {
        Task ExecuteAsync(List<DmCharge> charges);
    }
}
