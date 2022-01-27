using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Factories;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DMRunLogGateway : IDMRunLogGateway
    {
        private readonly DatabaseContext _context;

        public DMRunLogGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<DMRunLogDomain> GetDMRunLogByEntityNameAsync(string dynamoDbTableName)
        {
            var migrationRun = await _context.DMRunLogs
                .Where(x => x.DynamoDbTableName == dynamoDbTableName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return migrationRun.ToDomain();
        }

        public async Task<DMRunLogDomain> AddAsync(DMRunLogDomain migrationRunDomain)
        {

            var newDMRunLog = new DMRunLog()
            {
                DynamoDbTableName = migrationRunDomain.DynamoDbTableName,
                ExpectedRowsToMigrate = migrationRunDomain.ExpectedRowsToMigrate,
                ActualRowsMigrated = 0,
                StartRowId = 0,
                EndRowId = 0,
                LastRunDate = migrationRunDomain.LastRunDate,
                LastRunStatus = migrationRunDomain.LastRunStatus,
                UpdatedAt = DateTimeOffset.UtcNow,
                IsFeatureEnabled = true // not sure we need this attribute. May change to Active flag per migration run row?
            };

            await _context.DMRunLogs.AddAsync(newDMRunLog).ConfigureAwait(false);

            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1
                ? newDMRunLog.ToDomain()
                : null;

        }

        public async Task<bool> UpdateAsync(DMRunLogDomain dmRunLogDomain)
        {
            var dmLog = await _context.DMRunLogs.FirstOrDefaultAsync(x => x.Id == dmRunLogDomain.Id)
                .ConfigureAwait(false);

            if (dmLog == null)
                return false;

            dmLog.ActualRowsMigrated = dmRunLogDomain.ActualRowsMigrated;
            dmLog.StartRowId = dmRunLogDomain.StartRowId;
            dmLog.EndRowId = dmRunLogDomain.EndRowId;
            dmLog.ExpectedRowsToMigrate = dmRunLogDomain.ExpectedRowsToMigrate;
            dmLog.LastRunStatus = dmRunLogDomain.LastRunStatus;
            dmLog.UpdatedAt = DateTimeOffset.UtcNow;
            dmLog.IsFeatureEnabled = dmRunLogDomain.IsFeatureEnabled; // not sure we need this attribute. May change to Active flag per migration run row?
            return await _context.SaveChangesAsync().ConfigureAwait(false) == 1;
        }
    }
}
