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
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
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
        public async Task<IList<DmTransaction>> ListAsync()
        {
            var results = await _context.DmTransactionEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Transformed)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.ToDomain();
        }

        public async Task UpdateDMTransactionEntityItems(IList<DmTransaction> dMTransactionEntityDomainItems)
        {
            await _context.BulkUpdateAsync(dMTransactionEntityDomainItems.ToDatabase(), new BulkConfig { BatchSize = _batchSize }).ConfigureAwait(false);
        }

        public async Task<IList<DmTransaction>> GetTransformedListAsync()
        {
            var results = await _context.GetTransformedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<DmTransaction>> GetLoadedListAsync()
        {
            var results = await _context.GetLoadedListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<int> AddTransactionAsync(DmTransaction dmEntity)
        {
            await Task.Delay(0).ConfigureAwait(false);
            return -1;
        }
    }
}
