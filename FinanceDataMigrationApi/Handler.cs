using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Elasticsearch.Net;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.Accounts;
using FinanceDataMigrationApi.V1.UseCase.Asset;
using FinanceDataMigrationApi.V1.UseCase.Charges;
using FinanceDataMigrationApi.V1.UseCase.DmRunStatus;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Tenure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;
using FinanceDataMigrationApi.V1.UseCase.Tenure;
using FinanceDataMigrationApi.V1.UseCase.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FinanceDataMigrationApi
{
    public class Handler
    {
        readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        readonly IGetLastHintUseCase _getLastHintUseCase;
        readonly ITenureGetAllUseCase _tenureGetAllUseCase;
        readonly ITenureSaveToSqlUseCase _tenureSaveToSqlUseCase;
        readonly IAssetGetAllByElasticSearchUseCase _assetGetAllUseCase;
        readonly IAssetSaveToSqlUseCase _assetSaveToSqlUseCase;
        readonly IDmRunStatusGetUseCase _dmRunStatusGetUseCase;
        readonly IDmAssetRunStatusSaveUseCase _dmAssetRunStatusSaveUseCase;
        readonly IDmTenureRunStatusSaveUseCase _dmTenureRunStatusSaveUseCase;
        readonly IDmChargeExtractRunStatusSaveUseCase _dmChargeExtractRunStatusSaveUseCase;
        readonly IDmChargeLoadRunStatusSaveUseCase _dmChargeLoadRunStatusSaveUseCase;
        readonly ITimeLogSaveUseCase _timeLogSaveUseCase;
        readonly IExtractTransactionEntityUseCase _extractTransactionEntityUseCase;
        readonly ILoadTransactionEntityUseCase _loadTransactionEntityUseCase;
        readonly IDmTransactionExtractRunStatusSaveUseCase _dmTransactionExtractRunStatusSaveUseCase;
        readonly IDmTransactionLoadRunStatusSaveUseCase _dmTransactionLoadRunStatusSaveUseCase;
        readonly IExtractAccountEntityUseCase _extractAccountEntityUseCase;
        readonly ILoadAccountsUseCase _loadAccountsUseCase;
        readonly IDmAccountLoadRunStatusSaveUseCase _dmAccountLoadRunStatusSaveUseCase;

        private readonly IGetAllAssetsBySegmentScan _allAssetsBySegmentScan;
        /*readonly IDeleteAccountEntityUseCase _deleteAccountEntityUseCase;
        readonly IDeleteTransactionEntityUseCase _deleteTransactionEntityUseCase;*/
        readonly IIndexTransactionEntityUseCase _indexTransactionEntityUseCase;
        /*readonly IRemoveChargeTableUseCase _removeChargeTableUseCase;*/
        readonly int _waitDuration;

        private readonly int _batchSize;

        public Handler()
        {
            _waitDuration = int.Parse(Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "5");
            _batchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "100");

            DatabaseContext context = DatabaseContext.Create();

            IDMRunLogGateway migrationRunGateway = new DMRunLogGateway(context);
            IAmazonDynamoDB amazonDynamoDb = CreateAmazonDynamoDbClient();
            IDynamoDBContext dynamoDbContext = new DynamoDBContext(amazonDynamoDb);
            IChargeGateway chargeGateway = new ChargeGateway(context, amazonDynamoDb);
            ITransactionGateway transactionGateway = new TransactionGateway(context, amazonDynamoDb);
            ITenureGateway tenureGateway = new TenureGateway(context, amazonDynamoDb, dynamoDbContext);
            IAssetGateway assetGateway = new AssetGateway(context, amazonDynamoDb);
            IHitsGateway hitsGateway = new HitsGateway(context);
            IDmRunStatusGateway dmRunStatusGateway = new DmRunStatusGateway(context);
            ITimeLogGateway timeLogGateway = new TimeLogGateway(context);
            IDMRunLogGateway dmRunLogGateway = new DMRunLogGateway(context);
            IAccountsGateway accountsGateway = new AccountsGateway(context, amazonDynamoDb);
            IElasticClient elasticClient = CreateElasticClient();
            IEsGateway<QueryableTransaction> esTransactionGateway = new EsGateway<QueryableTransaction>(elasticClient, "Transactions");

            _getLastHintUseCase = new GetLastHintUseCase(hitsGateway);
            _loadChargeEntityUseCase = new LoadChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _extractChargeEntityUseCase = new ExtractChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _tenureGetAllUseCase = new TenureGetAllUseCase(tenureGateway);
            _tenureSaveToSqlUseCase = new TenureSaveToSqlUseCase(tenureGateway);
            _assetGetAllUseCase = new AssetGetAllByElasticSearchUseCase(assetGateway);
            _assetSaveToSqlUseCase = new AssetSaveToSqlUseCase(assetGateway);
            _dmRunStatusGetUseCase = new DmRunStatusGetUseCase(dmRunStatusGateway);
            _dmAssetRunStatusSaveUseCase = new DmAssetRunStatusSaveUseCase(dmRunStatusGateway);
            _dmTenureRunStatusSaveUseCase = new DmTenureRunStatusSaveUseCase(dmRunStatusGateway);
            _dmChargeExtractRunStatusSaveUseCase = new DmChargeExtractRunStatusSaveUseCase(dmRunStatusGateway);
            _dmChargeLoadRunStatusSaveUseCase = new DmChargeLoadRunStatusSaveUseCase(dmRunStatusGateway);
            _extractTransactionEntityUseCase = new ExtractTransactionEntityUseCase(dmRunLogGateway, transactionGateway);
            _loadTransactionEntityUseCase = new LoadTransactionEntityUseCase(transactionGateway);
            _dmTransactionExtractRunStatusSaveUseCase = new DmTransactionExtractRunStatusSaveUseCase(dmRunStatusGateway);
            _dmTransactionLoadRunStatusSaveUseCase = new DmTransactionLoadRunStatusSaveUseCase(dmRunStatusGateway);
            _extractAccountEntityUseCase = new ExtractAccountEntityUseCase(dmRunLogGateway, accountsGateway);
            _loadAccountsUseCase = new LoadAccountsUseCase(dmRunLogGateway, accountsGateway);
            _dmAccountLoadRunStatusSaveUseCase = new DmAccountLoadRunStatusSaveUseCase(dmRunStatusGateway);
            _timeLogSaveUseCase = new TimeLogSaveUseCase(timeLogGateway);
            _indexTransactionEntityUseCase = new IndexTransactionEntityUseCase(transactionGateway, esTransactionGateway);
            _allAssetsBySegmentScan = new GetAllAssetsBySegmentScan(assetGateway);
        }

        public async Task<StepResponse> ExtractTransactions()
        {
            try
            {
                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.AllTenureDmCompleted && runStatus.TransactionExtractDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(ExtractTransactions)}",
                        StartTime = DateTime.Now
                    };
                    await _extractTransactionEntityUseCase.ExecuteAsync().ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                }
                return new StepResponse() { Continue = false };
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ExtractTransactions)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> LoadTransactions()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("TRANSACTION_LOAD_BATCH_SIZE") ??
                              throw new Exception("Tenure download batch size is null."));
                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.TransactionExtractDate >= DateTime.Today && runStatus.TransactionLoadDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(LoadTransactions)}",
                        StartTime = DateTime.Now
                    };
                    var result = await _loadTransactionEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                    if (!result.Continue)
                        await _dmTransactionLoadRunStatusSaveUseCase.ExecuteAsync(DateTime.Today.AddYears(200)).ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(LoadTransactions)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> IndexTransactions()
        {
            try
            {
                var count = int.Parse(Environment.GetEnvironmentVariable("INDEX_BATCH_SIZE") ??
                            throw new Exception($"INDEX_BATCH_SIZE variable not found"));

                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.TransactionExtractDate >= DateTime.Today &&
                    runStatus.TransactionLoadDate >= DateTime.Today &&
                    runStatus.TransactionIndexDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(IndexTransactions)}",
                        StartTime = DateTime.Now
                    };
                    var result = await _indexTransactionEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                    if (!result.Continue)
                        await _dmTransactionLoadRunStatusSaveUseCase.ExecuteAsync(DateTime.Today).ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(IndexTransactions)} Exception:{e.GetFullMessage()}");
                throw;
            }
        }

        public async Task<StepResponse> LoadCharge()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("CHARGE_LOAD_BATCH_SIZE") ??
                                      throw new Exception("Tenure download batch size is null."));

                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.ChargeExtractDate >= DateTime.Today && runStatus.ChargeLoadDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(LoadCharge)}",
                        StartTime = DateTime.Now
                    };

                    var result = await _loadChargeEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                    if (!result.Continue)
                        await _dmChargeLoadRunStatusSaveUseCase.ExecuteAsync(DateTime.Today.AddYears(200)).ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(LoadCharge)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> ExtractCharge()
        {
            try
            {
                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.AllAssetDmCompleted && runStatus.ChargeExtractDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(ExtractCharge)}",
                        StartTime = DateTime.Now
                    };
                    await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                }
                return new StepResponse() { Continue = false };
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ExtractCharge)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> ExtractAccount()
        {
            try
            {
                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.AllTenureDmCompleted && runStatus.AccountExtractDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(ExtractAccount)}",
                        StartTime = DateTime.Now
                    };
                    await _extractAccountEntityUseCase.ExecuteAsync().ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                }
                return new StepResponse() { Continue = false };
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ExtractAccount)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> LoadAccount()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("ACCOUNT_LOAD_BATCH_SIZE") ??
                                      throw new Exception("Tenure download batch size is null."));

                var runStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);
                if (runStatus.AccountExtractDate >= DateTime.Today && runStatus.AccountLoadDate < DateTime.Today)
                {
                    DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                    {
                        ProcName = $"{nameof(LoadAccount)}",
                        StartTime = DateTime.Now
                    };

                    var result = await _loadAccountsUseCase.ExecuteAsync(count).ConfigureAwait(false);
                    await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);
                    if (!result.Continue)
                        await _dmAccountLoadRunStatusSaveUseCase.ExecuteAsync(DateTime.Today.AddYears(200)).ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(LoadCharge)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        /*        public async Task<StepResponse> DeleteAccount()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("ACCOUNT_LOAD_BATCH_SIZE") ??
                                      throw new Exception("Tenure download batch size is null."));

                DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(LoadAccount)}",
                    StartTime = DateTime.Now
                };

                var result = await _deleteAccountEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                return result;
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(LoadCharge)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> DeleteTransaction()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("TRANSACTION_LOAD_BATCH_SIZE") ??
                                      throw new Exception("Tenure download batch size is null."));

                DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(LoadAccount)}",
                    StartTime = DateTime.Now
                };

                var result = await _deleteTransactionEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                return result;
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(LoadCharge)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }*/

        public async Task<StepResponse> DownloadTenureToIfs()
        {
            try
            {
                var dmRunStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);

                if (dmRunStatus.AllTenureDmCompleted)
                    return new StepResponse() { Continue = false };

                var lastKey = await _getLastHintUseCase.ExecuteAsync("tenure").ConfigureAwait(false);

                Dictionary<string, AttributeValue> lastEvaluatedKey = new Dictionary<string, AttributeValue>
                {
                    {"id",new AttributeValue{S = lastKey.ToString()}}
                };

                DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_tenureGetAllUseCase)}",
                    StartTime = DateTime.Now
                };
                var response = await _tenureGetAllUseCase.ExecuteAsync(lastEvaluatedKey).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                lastEvaluatedKey = response.LastKey;

                if (response.TenureInformation.Count == 0 || lastEvaluatedKey.Count == 0)
                {
                    await _dmTenureRunStatusSaveUseCase.ExecuteAsync(true).ConfigureAwait(false);
                    return new StepResponse() { Continue = false };
                }

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"TenureInformation.ToXElement",
                    StartTime = DateTime.Now
                };
                var xmlData = response.TenureInformation.ToXElement();
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_tenureSaveToSqlUseCase)}",
                    StartTime = DateTime.Now
                };
                await _tenureSaveToSqlUseCase.ExecuteAsync(response.LastKey.Count > 0 ? lastEvaluatedKey["id"].S : lastKey.ToString(), xmlData).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(_waitDuration)
                };
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(DownloadTenureToIfs)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public async Task<StepResponse> DownloadAssets()
        {
            LoggingHandler.LogError($"Starting Assets Scan");
            var data = await _allAssetsBySegmentScan.ExecuteAsync().ConfigureAwait(false);
            LoggingHandler.LogError($"Completed Assets Scan count {data.Count}");

            return new StepResponse()
            {
                Continue = true,
                NextLambda = "SELF",
                NextStepTime = DateTime.Now.AddSeconds(_waitDuration)
            };
        }

        public async Task<StepResponse> DownloadAssetToIfs()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("ASSET_DOWNLOAD_BATCH_SIZE") ??
                                      throw new Exception("Asset download batch size is null."));

                var dmRunStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);

                if (dmRunStatus.AllAssetDmCompleted == true)
                    return new StepResponse() { Continue = false };

                var lastKey = await _getLastHintUseCase.ExecuteAsync("asset").ConfigureAwait(false);

                Dictionary<string, AttributeValue> lastEvaluatedKey = new Dictionary<string, AttributeValue>
                {
                    {"id",new AttributeValue{S = lastKey.ToString()}}
                };
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}" +
                                       $"{nameof(DownloadAssetToIfs)} Started with {count} Batch Size.");

                DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_assetGetAllUseCase)}",
                    StartTime = DateTime.Now
                };

                var response = await _assetGetAllUseCase.ExecuteAsync(count, lastEvaluatedKey).ConfigureAwait(false);

                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                lastEvaluatedKey = response.LastKey;

                if (response.Assets == null || response.Assets.Count == 0)
                {
                    await _dmAssetRunStatusSaveUseCase.ExecuteAsync(true).ConfigureAwait(false);
                    LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}" +
                                           $"{nameof(DownloadAssetToIfs)} All data downloaded.");
                    return new StepResponse() { Continue = false };
                }

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"Assets.ToXElement",
                    StartTime = DateTime.Now
                };
                var xmlData = response.Assets.ToXElement();
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_assetSaveToSqlUseCase)}",
                    StartTime = DateTime.Now
                };
                await _assetSaveToSqlUseCase.ExecuteAsync(response.LastKey.Count > 0 ? lastEvaluatedKey["id"].S : lastKey.ToString(), xmlData).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}" +
                                       $"{nameof(DownloadAssetToIfs)} Step finished.");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(_waitDuration)
                };
            }
            catch (Exception exception)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(DownloadAssetToIfs)} Exception: {exception.GetFullMessage()}");
                return new StepResponse()
                {
                    Continue = false
                };
            }
        }

        public static AmazonDynamoDBClient CreateAmazonDynamoDbClient()
        {
            bool result = bool.Parse(value: Environment.GetEnvironmentVariable("DynamoDb_LocalMode") ?? "false");
            if (result)
            {
                string url = Environment.GetEnvironmentVariable("DynamoDb_LocalServiceUrl");
                return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                {
                    ServiceURL = url
                });
            }
            return new AmazonDynamoDBClient();
        }

        public static ElasticClient CreateElasticClient()
        {
            var url = Environment.GetEnvironmentVariable("ELASTICSEARCH_DOMAIN_URL");
            if (string.IsNullOrEmpty(url))
                url = "http://localhost:9200/";

            var pool = new SingleNodeConnectionPool(new Uri(url));
            var connectionSettings = new ConnectionSettings(pool)
                .PrettyJson()
                .ThrowExceptions()
                .DisableDirectStreaming();
            return new ElasticClient(connectionSettings);
        }
    }
}
