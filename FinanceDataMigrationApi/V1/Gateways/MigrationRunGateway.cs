using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Factories;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class MigrationRunGateway : IMigrationRunGateway
    {
        private readonly DatabaseContext _context;

        public MigrationRunGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<DMRunLogDomain> GetDMRunLogByEntityNameAsync(string dynamoDbTableName)
        {
            try
            {
                var migrationRun = await _context.MigrationRuns
                    .Where(x => x.DynamoDbTableName == dynamoDbTableName)
                    .OrderByDescending(x => x.Id)
                    .FirstAsync().ConfigureAwait(false);

                return migrationRun.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task AddAsync (DMRunLogDomain migrationRunDomain)
        {
            var migrationRun = new DMRunLog()
            {
                DynamoDbTableName = migrationRunDomain.DynamoDbTableName,
                ExpectedRowsToMigrate = migrationRunDomain.ExpectedRowsToMigrate,
                ActualRowsMigrated = migrationRunDomain.ActualRowsMigrated,
                StartRowId = migrationRunDomain.StartRowId,
                EndRowId = migrationRunDomain.EndRowId,
                LastRunDate = migrationRunDomain.LastRunDate,
                LastRunStatus = migrationRunDomain.LastRunStatus,
                UpdatedAt = DateTimeOffset.UtcNow,
                IsFeatureEnabled = true // not sure we need this attribute. May change to Active flag per migration run row?
            };

            await _context.MigrationRuns.AddAsync(migrationRun).ConfigureAwait(false); 
        }

        public async Task UpdateAsync(DMRunLogDomain migrationRunDomain)
        {
            var migrationRun = await _context.MigrationRuns.FirstOrDefaultAsync(x => x.Id == migrationRunDomain.Id).ConfigureAwait(false);

            migrationRun.IsFeatureEnabled = true; // not sure we need this attribute. May change to Active flag per migration run row?
            migrationRun.LastRunStatus = migrationRunDomain.LastRunStatus;
            migrationRun.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
