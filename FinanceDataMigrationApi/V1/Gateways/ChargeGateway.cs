using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoMapper.Internal;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class ChargeGateway : IChargeGateway
    {
        private readonly DatabaseContext _context;
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public ChargeGateway(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb)
        {
            _context = context;
            _amazonDynamoDb = amazonDynamoDb;
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

        public async Task<IList<Charge>> GetExtractedListAsync(int count)
        {
            var results = await _context.GetExtractedChargeListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Extracted);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<IList<Charge>> GetTransformedListAsync(int count)
        {
            var results = await _context.GetTransformedChargeListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Transformed);
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
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderCharge).ConfigureAwait(false);

                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

                _context.ChargesDbEntities.Where(p =>
                    charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.Loaded);
                await _context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (ResourceNotFoundException rnf)
            {
                LoggingHandler.LogError($"One of the table involved in the account is not found: {rnf.Message}");
                _context.ChargesDbEntities.Where(p =>
                        charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogError($"Internal Server Error: {ise.Message}");
                _context.ChargesDbEntities.Where(p =>
                        charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogError($"Transaction Canceled: {tce.Message}");
                _context.ChargesDbEntities.Where(p =>
                        charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                _context.ChargesDbEntities.Where(p =>
                        charges.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}
