using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class DMRunLogFactory
    {
        //TODO NM: use AutoMapper for this stuff!!!! 

        public static DMRunLog ToDatabase(this DMRunLogDomain dMRunLogDomain)
        {
            return dMRunLogDomain == null ? null : new DMRunLog
            {
                Id = dMRunLogDomain.Id,
                DynamoDbTableName = dMRunLogDomain.DynamoDbTableName,
                ExpectedRowsToMigrate = dMRunLogDomain.ExpectedRowsToMigrate,
                ActualRowsMigrated = dMRunLogDomain.ActualRowsMigrated,
                StartRowId = dMRunLogDomain.StartRowId,
                EndRowId = dMRunLogDomain.EndRowId,
                LastRunDate = dMRunLogDomain.LastRunDate,
                LastRunStatus = dMRunLogDomain.LastRunStatus,
                UpdatedAt = dMRunLogDomain.UpdatedAt,
                IsFeatureEnabled = dMRunLogDomain.IsFeatureEnabled
            };
        }

        public static DMRunLogDomain ToDomain(this DMRunLog dMRunLog)
        {
            return dMRunLog == null ? null : new DMRunLogDomain
            {
                Id = dMRunLog.Id,
                DynamoDbTableName = dMRunLog.DynamoDbTableName,
                ExpectedRowsToMigrate = dMRunLog.ExpectedRowsToMigrate,
                ActualRowsMigrated = dMRunLog.ActualRowsMigrated,
                StartRowId = dMRunLog.StartRowId,
                EndRowId = dMRunLog.EndRowId,
                LastRunDate = dMRunLog.LastRunDate,
                LastRunStatus = dMRunLog.LastRunStatus,
                UpdatedAt = dMRunLog.UpdatedAt,
                IsFeatureEnabled = dMRunLog.IsFeatureEnabled
            };
        }

        public static DMRunLog ToDomain(this MigrationRunUpdateRequest migrationRunUpdateRequest)
        {
            return migrationRunUpdateRequest == null ? null : new DMRunLog
            {
                DynamoDbTableName = migrationRunUpdateRequest.DynamoDbEntity,
                ExpectedRowsToMigrate = migrationRunUpdateRequest.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRunUpdateRequest.ActualRowsMigrated,
                StartRowId = migrationRunUpdateRequest.StartRowId,
                EndRowId = migrationRunUpdateRequest.EndRowId,
                LastRunDate = migrationRunUpdateRequest.LastRunDate,
                LastRunStatus = migrationRunUpdateRequest.LastRunStatus.ToString(),
                UpdatedAt = migrationRunUpdateRequest.UpdatedAt,
                IsFeatureEnabled = migrationRunUpdateRequest.IsFeatureEnabled
            };
        }

        //public static List<MigrationRun> ToDomain(this IEnumerable<MigrationRun> databaseEntity)
        //{
        //    return databaseEntity.Select(p => p.ToDomain())
        //                         .OrderBy(x => x.LastRunDate)
        //                         .ToList();
        //}


    }
}
