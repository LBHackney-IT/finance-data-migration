using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class MigrationRunFactory
    {
        //TODO NM: use AutoMapper for this stuff!!!! 

        public static DMRunLog ToDatabase(this DMRunLogDomain migrationRun)
        {
            return migrationRun == null ? null : new DMRunLog
            {
                Id = migrationRun.Id,
                DynamoDbTableName = migrationRun.DynamoDbTableName,
                ExpectedRowsToMigrate = migrationRun.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRun.ActualRowsMigrated,
                StartRowId = migrationRun.StartRowId,
                EndRowId = migrationRun.EndRowId,
                LastRunDate = migrationRun.LastRunDate,
                LastRunStatus = migrationRun.LastRunStatus,
                UpdatedAt = migrationRun.UpdatedAt
            };
        }

        public static DMRunLogDomain ToDomain(this DMRunLog migrationRunDomain)
        {
            return migrationRunDomain  == null ? null : new DMRunLogDomain
            {
                Id = migrationRunDomain.Id,
                DynamoDbTableName = migrationRunDomain.DynamoDbTableName,
                ExpectedRowsToMigrate = migrationRunDomain.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRunDomain.ActualRowsMigrated,
                StartRowId = migrationRunDomain.StartRowId,
                EndRowId = migrationRunDomain.EndRowId,
                LastRunDate = migrationRunDomain.LastRunDate,
                LastRunStatus = migrationRunDomain.LastRunStatus,
                UpdatedAt = migrationRunDomain.UpdatedAt
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
                UpdatedAt = migrationRunUpdateRequest.UpdatedAt 
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
