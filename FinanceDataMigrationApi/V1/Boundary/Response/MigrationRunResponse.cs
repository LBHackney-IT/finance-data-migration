using FinanceDataMigrationApi.V1.Domain;
using System;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class MigrationRunResponse
    {
        public Guid Id { get; set; }
        public string DynamoDbEntity { get; set; }
        public long ExpectedRowsToMigrate { get; set; }
        public long ActualRowsMigrated { get; set; }
        public long StartRowId { get; set; }
        public long EndRowId { get; set; }
        public DateTime LastRunDate { get; set; }
        public MigrationRunStatus LastRunStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
