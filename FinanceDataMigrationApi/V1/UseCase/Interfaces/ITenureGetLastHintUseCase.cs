using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITenureGetLastHintUseCase
    {
        public Task<Guid> ExecuteAsync();
    }
}
