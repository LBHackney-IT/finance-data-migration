using FinanceDataMigrationApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetMigrationRunByIdUseCase
    {
        public Task<MigrationRunResponse> ExecuteAsync(Guid id);
    }
}
