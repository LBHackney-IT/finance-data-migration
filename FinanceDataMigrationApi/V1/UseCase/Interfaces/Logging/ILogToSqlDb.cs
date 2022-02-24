using Elasticsearch.Net.Specification.SecurityApi;
using FinanceDataMigrationApi.V1.Domain.Logging;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Logging
{
    public interface ILogToSqlDb
    {
        public void ExecuteAsync(LogRequest request);
    }
}
