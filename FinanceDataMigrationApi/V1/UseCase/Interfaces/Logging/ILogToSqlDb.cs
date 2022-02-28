using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Logging
{
    public interface ILogToSqlDb
    {
        public Task ExecuteAsync(LogRequest request);
    }
}
