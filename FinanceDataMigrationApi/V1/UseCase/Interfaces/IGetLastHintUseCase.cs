using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetLastHintUseCase
    {
        public Task<Guid> ExecuteAsync(string tableName);
    }
}
