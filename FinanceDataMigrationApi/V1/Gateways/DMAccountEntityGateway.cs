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

        /// <summary>
        /// Extract the Accounts Entities to migrate asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Accounts Entities.
        /// </returns>
        //public async Task<int> ExtractAsync(DateTime? processingDate)
        public async Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            try
            {
                return await _context.ExtractDMAccountsAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
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
