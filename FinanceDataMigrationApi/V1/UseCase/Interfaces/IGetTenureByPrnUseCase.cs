using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetTenureByPrnUseCase
    {
        public Task<List<TenureInformation>> ExecuteAsync(string prn);
    }
}
