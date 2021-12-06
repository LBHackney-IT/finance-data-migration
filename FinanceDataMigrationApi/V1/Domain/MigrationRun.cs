using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class MigrationRun
    {
        public Guid Id { get; set; }
        [Required]
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
