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
using Microsoft.EntityFrameworkCore;

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
            return await _context.ExtractDMTransactionsAsync(processingDate).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the Transaction Entities to migrate asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Transaction Entities.
        /// </returns>
        public async Task<IList<DMTransactionEntityDomain>> ListAsync()
        {
            var results = await _context.DMTransactionEntities
                .Where(x => x.IsTransformed == false)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.ToDomain();
        }

        public async Task UpdateDMTransactionEntityItems(IList<DMTransactionEntityDomain> dMTransactionEntityDomainItems)
        {
            await _context.BulkUpdateAsync(dMTransactionEntityDomainItems.ToDatabase(), new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
        }

        public async Task<IList<DMTransactionEntityDomain>> GetTransformedListAsync()
        {
            var results = await _context.GetTransformedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<DMTransactionEntityDomain>> GetLoadedListAsync()
        {
            var results = await _context.GetLoadedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<int> AddTransactionAsync(DMTransactionEntityDomain dmEntity)
        {
            await Task.Delay(0).ConfigureAwait(false);
            return -1;
        }
    }
}
