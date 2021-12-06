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

        public static MigrationRunDbEntity ToDatabase(this MigrationRun migrationRun)
        {
            return migrationRun == null ? null : new MigrationRunDbEntity
            {
                Id = migrationRun.Id,
                DynamoDbEntity = migrationRun.DynamoDbEntity,
                ExpectedRowsToMigrate = migrationRun.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRun.ActualRowsMigrated,
                StartRowId = migrationRun.StartRowId,
                EndRowId = migrationRun.EndRowId,
                LastRunDate = migrationRun.LastRunDate,
                LastRunStatus = migrationRun.LastRunStatus,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static MigrationRun ToDomain(this MigrationRunDbEntity migrationRunDbEntity)
        {
            return migrationRunDbEntity == null ? null : new MigrationRun
            {
                Id = migrationRunDbEntity.Id,
                DynamoDbEntity = migrationRunDbEntity.DynamoDbEntity,
                ExpectedRowsToMigrate = migrationRunDbEntity.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRunDbEntity.ActualRowsMigrated,
                StartRowId = migrationRunDbEntity.StartRowId,
                EndRowId = migrationRunDbEntity.EndRowId,
                LastRunDate = migrationRunDbEntity.LastRunDate,
                LastRunStatus = migrationRunDbEntity.LastRunStatus,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static MigrationRun ToDomain(this MigrationRunUpdateRequest migrationRunUpdateRequest)
        {
            return migrationRunUpdateRequest == null ? null : new MigrationRun
            {
                DynamoDbEntity = migrationRunUpdateRequest.DynamoDbEntity,
                ExpectedRowsToMigrate = migrationRunUpdateRequest.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRunUpdateRequest.ActualRowsMigrated,
                StartRowId = migrationRunUpdateRequest.StartRowId,
                EndRowId = migrationRunUpdateRequest.EndRowId,
                LastRunDate = migrationRunUpdateRequest.LastRunDate,
                LastRunStatus = migrationRunUpdateRequest.LastRunStatus,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static List<MigrationRun> ToDomain(this IEnumerable<MigrationRunDbEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain())
                                 .OrderBy(x => x.LastRunDate)
                                 .ToList();
        }


    }
}
