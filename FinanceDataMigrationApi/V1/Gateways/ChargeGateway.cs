using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoMapper.Internal;
using EFCore.BulkExtensions;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class ChargeGateway : IChargeGateway
    {
        private readonly DatabaseContext _context;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<IChargeGateway> _logger;

        public ChargeGateway(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb, ILogger<IChargeGateway> logger)
        {
            _context = context;
            _amazonDynamoDb = amazonDynamoDb;
            _logger = logger;
        }

        /// <summary>
        /// Extracts the Charge Entities to migrate async
        /// </summary>
        /// <param name="processingDate">date of processing</param>
        /// <returns>number of records extracted</returns>
        public async Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            if (processingDate == null) throw new ArgumentNullException(nameof(processingDate));
            return await _context.ExtractDMChargesAsync().ConfigureAwait(false);
        }


        /*public async Task<List<DmDetailedChargesEntity>> GetDetailChargesListAsync(string paymentReference)
        {
            return await _context.GetDetailChargesListAsync(paymentReference).ConfigureAwait(false);
        }

        /// <summary>
        /// List Charge entities to migrate
        /// </summary>
        /// <returns>List of Charge</returns>
        public async Task<IList<DMChargeEntityDomain>> ListAsync()
        {
            var results = await _context.DMChargeEntities
                .Where(x => x.MigrationStatus==EMigrationStatus.Transformed)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.ToDomain();
        }

        public async Task UpdateDMChargeEntityItems(IList<DMChargeEntityDomain> dMChargeEntityDomainItems)
        {
            await _context
                .BulkUpdateAsync(dMChargeEntityDomainItems.ToDatabase(), new BulkConfig { BatchSize = _batchSize })
                .ConfigureAwait(false);
        }



        public async Task<IList<DMChargeEntityDomain>> GetLoadedListAsync()
        {
            var results = await _context.GetLoadedChargeListAsync().ConfigureAwait(false);
            return results.ToDomain();
        }


        public async Task<int> AddChargeAsync(DMChargeEntityDomain dmEntity)
        {
            await Task.Delay(0).ConfigureAwait(false);
            return -1;
        }*/

        public async Task<IList<Charge>> GetTransformedListAsync()
        {
            var results = await _context.GetTransformedChargeListAsync().ConfigureAwait(false);
            results.ToList().ForEach(p=>p.MigrationStatus=EMigrationStatus.Loading);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task BatchInsert(List<Charge> charges)
        {
            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (Charge charge in charges)
            {
                var columns = charge.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "Charges",
                        Item = columns,
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                        ConditionExpression = "attribute_not_exists(target_id)"
                    }
                });
            }

            TransactWriteItemsRequest placeOrderCharge = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            try
            {
                await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);
                _context.ChargesDbEntities.Where(p=>
                    charges.Select(i=>i.Id).Contains(p.Id)).
                    ForAll(p=>p.MigrationStatus=EMigrationStatus.Loaded);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _context.ChargesDbEntities.Where(p =>
                        charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}
