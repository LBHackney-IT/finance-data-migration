using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IUpdateUseCase
    {
        public Task<MigrationRunResponse> ExecuteAsync(MigrationRunUpdateRequest migrationRun, Guid id);
    }
}
