using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;
using System;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    [DynamoDBTable("MigrationRuns", LowerCamelCaseProperties = true)]
    public class MigrationRunDbEntity
    {
        [DynamoDBHashKey]
        [DynamoDBProperty(AttributeName = "id")]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "dynamodb_entity")]
        public string DynamoDbEntity { get; set; }

        [DynamoDBProperty(AttributeName = "expected_rows_to_migrate")]
        public long ExpectedRowsToMigrate { get; set; }

        [DynamoDBProperty(AttributeName = "actual_rows_migrated")]
        public long ActualRowsMigrated { get; set; }

        [DynamoDBProperty(AttributeName = "start_row_id")]
        public long StartRowId { get; set; }

        [DynamoDBProperty(AttributeName = "end_row_id")]
        public long EndRowId { get; set; }

        [DynamoDBProperty(AttributeName = "last_run_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime LastRunDate { get; set; }

        [DynamoDBProperty(AttributeName = "last_run_status", Converter = typeof(DynamoDbEnumConverter<MigrationRunStatus>))]
        public MigrationRunStatus LastRunStatus { get; set; }

        [DynamoDBProperty(AttributeName = "updated_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }
    }
}
