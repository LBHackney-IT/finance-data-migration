using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using EFCore.BulkExtensions;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class ChargeGateway : IChargeGateway
    {
        private readonly DatabaseContext _context;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<IChargeGateway> _logger;

        private readonly int _batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE"));

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
            try
            {
                return await _context.ExtractDMChargesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }


        public async Task<List<DMDetailedChargesEntity>> GetDetailChargesListAsync(string paymentReference)
        {
            try
            {
                return await _context.GetDetailChargesListAsync(paymentReference).ConfigureAwait(false);

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
                    .BulkUpdateAsync(dMChargeEntityDomainItems.ToDatabase(), new BulkConfig { BatchSize = _batchSize })
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

        public async Task<bool> BatchInsert(List<Charge> charges)
        {
            bool result = false;
            List<TransactWriteItem> actions = new List<TransactWriteItem>();
            foreach (Charge charge in charges)
            {
                Dictionary<string, AttributeValue> columns = new Dictionary<string, AttributeValue>();
                columns = charge.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "Charges",
                        Item = columns,
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                        ConditionExpression = "attribute_not_exists(id)"
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
                result = true;
            }
            catch (ResourceNotFoundException rnf)
            {
                _logger.LogDebug($"One of the table involved in the charge is not found: {rnf.Message}");
            }
            catch (InternalServerErrorException ise)
            {
                _logger.LogDebug($"Internal Server Error: {ise.Message}");
            }
            catch (TransactionCanceledException tce)
            {
                _logger.LogDebug($"Transaction Canceled: {tce.Message}");
            }

            return result;
        }
    }
}