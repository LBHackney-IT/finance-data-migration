using EFCore.BulkExtensions;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DMTransactionEntityGateway : IDMTransactionEntityGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public DMTransactionEntityGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Extract the Transaction Entities to migrate asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Transaction Entities.
        /// </returns>
        //public async Task<int> ExtractAsync(DateTime? processingDate)
        public async Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            try
            {
                return await _context.ExtractDMTransactionsAsync(processingDate).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Lists the Transaction Entities to migrate asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Transaction Entities.
        /// </returns>
        public async Task<IList<DMTransactionEntityDomain>> ListAsync()
        {
            try
            {
                var results = await _context.GetDMTransactionEntitiesAsync().ConfigureAwait(false);

                return results.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task UpdateDMTransactionEntityItems(IList<DMTransactionEntityDomain> dMTransactionEntityDomainItems)
        {
            try
            {
                await _context.BulkUpdateAsync(dMTransactionEntityDomainItems.ToDatabase(), new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<DMTransactionEntityDomain>> GetTransformedListAsync()
        {
            try
            {
                var results = await _context.GetTransformedListAsync().ConfigureAwait(false);

                return results.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<int> AddTransactionAsync(DMTransactionEntityDomain dmEntity)
        {
            // TODO
            try
            {
                await Task.Delay(0).ConfigureAwait(false);
                return -1;
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
