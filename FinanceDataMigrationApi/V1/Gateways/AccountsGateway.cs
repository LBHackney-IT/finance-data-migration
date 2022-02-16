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
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var results = await _context.AccountDbEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Extracted)
                .Take(count)
                .Include(p => p.ConsolidatedCharges)
                .Include(t => t.Tenure)
                .Include(t => t.Tenure.PrimaryTenants)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

                results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Loading);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return results.ToDomain();
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(AccountsGateway)}.{nameof(GetExtractedListAsync)} Exception: {ex.GetFullMessage()}");
                throw;
            }
        }

        public async Task<IList<DmAccount>> GetLoadFailedListAsync(int count)
        {
            var results = await _context.AccountDbEntities
                .Where(p => p.MigrationStatus == EMigrationStatus.LoadFailed)
                .Take(count)
                .Include(c => c.ConsolidatedCharges)
                .Include(t => t.Tenure)
                .Include(p => p.Tenure.PrimaryTenants)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

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
                if (account.TargetId == null)
                    continue;

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

        public async Task BatchDelete(List<DmAccount> accounts)
        {
            DatabaseContext context = DatabaseContext.Create();

            if (accounts == null || !accounts.Any())
            {
                LoggingHandler.LogError("There is no accounts to delete from DynamoDm");
                throw new ArgumentNullException(nameof(accounts));
            }

            List<TransactWriteItem> actions = new List<TransactWriteItem>(accounts.Count);
            foreach (DmAccount account in accounts)
            {

                if (account.Tenure?.PrimaryTenants != null)
                {

                    Dictionary<string, AttributeValue> columns = account.ToQueryRequest();

                    actions.Add(new TransactWriteItem
                    {
                        /*Delete = new Delete()
                        {
                            TableName = "Accounts",
                            Key = new Dictionary<string, AttributeValue>
                            {
                                {"id",new AttributeValue(account.Id.ToString())}
                            },
                            ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD
                        },*/
                        Update = new Update()
                        {
                            TableName = "Accounts",
                            Key =
                                new Dictionary<string, AttributeValue> { { "id", new AttributeValue(account.Id.ToString()) } },
                            ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD,
                            ExpressionAttributeNames = new Dictionary<string, string>()
                            {
                                { "#P", "primaryTenants" }/*,
                                {"#ID","id"}*/
                            },
                            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                            {
                                {
                                    ":newPT", new AttributeValue()
                                    {
                                        L = account.Tenure.PrimaryTenants.Select(p =>
                                            new AttributeValue
                                            {
                                                M = new Dictionary<string, AttributeValue>
                                                {
                                                    {"id", new AttributeValue(p.Id.ToString())},
                                                    {"fullName", new AttributeValue(p.FullName?.ToString() ?? "")}
                                                }
                                            }
                                        ).ToList()
                                    }
                                }/*,
                                {
                                    ":newID", new AttributeValue(account.DynamoDbId.ToString())
                                }*/
                            },
                            UpdateExpression = "SET #P = :newPT"
                        }
                    });
                }
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
                    ForAll(p => p.MigrationStatus = EMigrationStatus.Deleted);
                await context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (ResourceNotFoundException rnf)
            {
                LoggingHandler.LogError($"One of the table involved in the account is not found: {rnf.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.DeleteFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (InternalServerErrorException ise)
            {
                LoggingHandler.LogError($"Internal Server Error: {ise.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.DeleteFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (TransactionCanceledException tce)
            {
                LoggingHandler.LogError($"Transaction Canceled: {tce.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.DeleteFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"TransactWriteItemsAsync: {ex.Message}");
                context.AccountDbEntities.Where(p =>
                        accounts.Select(i => i.Id).Contains(p.Id)).
                    ForAll(p => p.MigrationStatus = EMigrationStatus.DeleteFailed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<List<DmAccount>> GetLoadedListAsync(int count)
        {
            var results = await _context.AccountDbEntities
                .Where(p => p.MigrationStatus == EMigrationStatus.LoadFailed)
                .Take(count)
                .Include(c => c.ConsolidatedCharges)
                .Include(t => t.Tenure)
                .Include(p => p.Tenure.PrimaryTenants)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

            return results?.ToDomain();
        }

        public async Task<List<DmAccount>> GetLoadedListForDeleteAsync(int count)
        {
            var results = await _context.AccountDbEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.Loaded)
                .Take(count)
                .Include(p => p.ConsolidatedCharges)
                .Include(t => t.Tenure)
                .Include(t => t.Tenure.PrimaryTenants)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Deleting);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public async Task<List<DmAccount>> GetToBeDeletedListForDeleteAsync(int count)
        {
            var results = await _context.AccountDbEntities
                .Where(x => x.MigrationStatus == EMigrationStatus.ToBeDeleted)
                .Take(count)
                .Include(p => p.ConsolidatedCharges)
                .Include(t => t.Tenure)
                .Include(t => t.Tenure.PrimaryTenants)
                .ToListWithNoLockAsync()
                .ConfigureAwait(false);

            results.ToList().ForAll(p => p.MigrationStatus = EMigrationStatus.Deleting);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return results.ToDomain();
        }

        public Task UpdateDMAccountEntityItems(object loadedAccounts)
        {
            throw new NotImplementedException();
        }
    }
}
