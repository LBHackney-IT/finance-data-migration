using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;

namespace FinanceDataMigrationApi.V1.Boundary.Request
{
    public class MigrationRunUpdateRequest
    {
        public string DynamoDbEntity { get; set; }
        public long ExpectedRowsToMigrate { get; set; }
        public long ActualRowsMigrated { get; set; }
        public long StartRowId { get; set; }
        public long EndRowId { get; set; }
        public DateTime LastRunDate { get; set; }

        [AllowedValues(typeof(MigrationRunStatus))]
        public MigrationRunStatus LastRunStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
