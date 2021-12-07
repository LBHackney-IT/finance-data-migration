using FinanceDataMigrationApi.V1.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    /// <summary>
    /// The Data Migration Run Log.
    /// </summary>
    [Table("DMRunLog")]
    public class DMRunLog 
    {
        [Key]
        public long Id { get; set; }

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
