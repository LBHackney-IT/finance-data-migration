using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetMigrationRunListUseCase
    {
        Task<List<DMRunLogDomain>> ExecuteAsync();
    }
}
