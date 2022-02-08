using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain.Accounts;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using AutoMapper.Internal;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class AccountsGateway : IAccountsGateway
    {
        private readonly DatabaseContext _context;
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public AccountsGateway(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb)
        {
            _context = context;
            _amazonDynamoDb = amazonDynamoDb;
        }

        public async Task<int> ExtractAsync()
        {
            return await _context.ExtractDmAccountsAsync().ConfigureAwait(false);
        }

        public async Task<IList<DmAccount>> GetExtractedListAsync(int count)
        {
            var results = await _context.GetExtractedAccountListAsync(count).ConfigureAwait(false);
            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Loading);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task BatchInsert(List<DmAccount> accounts)
        {
            DatabaseContext context = DatabaseContext.Create();

            if (accounts == null || !accounts.Any())
            {
                LoggingHandler.LogError("There is no accounts to save in DynamoDm");
                throw new ArgumentNullException(nameof(accounts));
            }

            List<TransactWriteItem> actions = new List<TransactWriteItem>(accounts.Count);
            foreach (DmAccount account in accounts)
            {
                Dictionary<string, AttributeValue> columns = account.ToQueryRequest();

                actions.Add(new TransactWriteItem
                {
                    Put = new Put()
                    {
                        TableName = "Accounts",
                        Item = columns,
                        ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                        ConditionExpression = "attribute_not_exists(id)"
                    }
                });
            }

            TransactWriteItemsRequest placeOrderAccount = new TransactWriteItemsRequest
            {
                TransactItems = actions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            try
            {
                var writeResult = await _amazonDynamoDb.TransactWriteItemsAsync(placeOrderAccount).ConfigureAwait(false);

                if (writeResult.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception(writeResult.ResponseMetadata.ToString());

                context.AccountDbEntities.Where(p =>
                    accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.Loaded);
                await context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (ResourceNotFoundException rnf)
            {
                LoggingHandler.LogError($"One of the table involved in the account is not found: {rnf.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogError($"Internal Server Error: {ise.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogError($"Transaction Canceled: {tce.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.LoadFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public Task<List<DmAccountDbEntity>> GetLoadedListAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateDMAccountEntityItems(object loadedAccounts)
        {
            throw new NotImplementedException();
        }
    }
}
