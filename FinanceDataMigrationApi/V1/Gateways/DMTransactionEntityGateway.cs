using FinanceDataMigrationApi.V1.Gateways.Interfaces;
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
        public async Task<int> ExtractAsync(DateTime? processingDate)
        {
            return await _context.ExtractDMTransactionsAsync(processingDate).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the Transaction Entities to migrate asynchronous.
        /// </summary>
        /// <returns>
        /// The list of Transaction Entities.
        /// </returns>
        public async Task<IList<DMTransactionEntity>> ListAsync()
        {
            var results = await _context.GetDMTransactionEntitiesAsync().ConfigureAwait(false);

            return results;
        }

        public async Task<DMTransactionEntity> GetDMTransactionEntityByIdAsync(int id)
        {
            await Task.Delay(0).ConfigureAwait(false);
            //try
            //{
            //    var result = await _context.DMTransactionEntitiesValue.FirstOrDefault(x => x.Id == id) ;

            //    return results.ToDomain;

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            return new DMTransactionEntity();
        }


        public async Task UpdateDMTransactionEntities(DMTransactionEntity dMTransactionEntity)
        {
            await Task.Delay(0).ConfigureAwait(false);
            //throw new NotImplementedException();
        }

        public async Task<int> UpdateDMTransactionEntityItems(IList<DMTransactionEntity> items)
        {
            await Task.Delay(0).ConfigureAwait(false);
            //throw new NotImplementedException();
            return 0;
        }
    }
}
