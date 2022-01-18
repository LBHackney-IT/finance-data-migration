using FinanceDataMigrationApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetTenureInformationByPrnUseCase
    {
        public Task<TenureInformation> ExecuteAsync(string prn);
    }
}
