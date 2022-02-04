using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITimeLogSaveUseCase
    {
        Task ExecuteAsync(DmTimeLogModel timeLogModel);
    }
}
