using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IAssetGetLastHintUseCase
    {
        public Task<Guid> ExecuteAsync();
    }
}
