using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DMChargeEntityGateway : IDMChargeEntityGateway
    {
        private readonly DatabaseContext _context;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

        public DMChargeEntityGateway(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Extracts the Charge Entities to migrate async
        /// </summary>
        /// <param name="processingDate">date of processing</param>
        /// <returns>number of records extracted</returns>
        public async Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            try
            {
                return await _context.ExtractDMChargesAsync(processingDate).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// List Charge entities to migrate
        /// </summary>
        /// <returns>List of Charge</returns>
        public async Task<IList<DMChargeEntityDomain>> ListAsync()
        {
            try
            {
                var results = await _context.DMChargeEntities
                    .Where(x => x.IsTransformed == false)
                    .ToListAsync()
                    .ConfigureAwait(false);

                return results.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task UpdateDMChargeEntityItems(IList<DMChargeEntityDomain> dMChargeEntityDomainItems)
        {
            try
            {
                await _context
                    .BulkUpdateAsync(dMChargeEntityDomainItems.ToDatabase(), new BulkConfig {BatchSize = _batchSize})
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<DMChargeEntityDomain>> GetTransformedListAsync()
        {
            try
            {
                var results = await _context.GetTransformedChargeListAsync().ConfigureAwait(false);

                return results.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<IList<DMChargeEntityDomain>> GetLoadedListAsync()
        {
            try
            {
                var results = await _context.GetLoadedChargeListAsync().ConfigureAwait(false);

                return results.ToDomain();
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public async Task<int> AddChargeAsync(DMChargeEntityDomain dmEntity)
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
