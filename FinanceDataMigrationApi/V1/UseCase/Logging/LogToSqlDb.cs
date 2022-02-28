using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Logging;

namespace FinanceDataMigrationApi.V1.UseCase.Logging
{
    public class LogToSqlDb: ILogToSqlDb
    {
        private DatabaseContext _context;
        public async Task ExecuteAsync(LogRequest request)
        {
            _context = DatabaseContext.Create();
            DmLog
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
