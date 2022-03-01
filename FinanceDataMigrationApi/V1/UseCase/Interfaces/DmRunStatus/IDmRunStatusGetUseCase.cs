using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus
{
    public interface IDmRunStatusGetUseCase
    {
        public Task<DmRunStatusModel> ExecuteAsync();
    }
}
