using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DMRunLogDomain
    {
        public long Id { get; set; }

        [Required]
        public string DynamoDbTableName { get; set; }
        public long ExpectedRowsToMigrate { get; set; }
        public long ActualRowsMigrated { get; set; }
        public long StartRowId { get; set; }
        public long EndRowId { get; set; }
        public DateTimeOffset? LastRunDate { get; set; }

        public string LastRunStatus { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsFeatureEnabled { get; set; }
    }
}
