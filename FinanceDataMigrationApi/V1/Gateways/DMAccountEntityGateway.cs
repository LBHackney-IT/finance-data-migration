using EFCore.BulkExtensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DMAccountEntityGateway : IDMAccountEntityGateway
    {
        private readonly DbAccountsContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public DMAccountEntityGateway(DbAccountsContext context)
        {
            _context = context;
        }

        public async Task<IList<DMAccountEntity>> GetLoadedListAsync()
        {
            try
            {
                var results = await _context.GetLoadedListAsync().ConfigureAwait(false);

                return results;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task UpdateDMAccountEntityItems(IList<DMAccountEntity> dMAccountEntities)
        {
            try
            {
                await _context.BulkUpdateAsync(dMAccountEntities, new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }
    }
}
