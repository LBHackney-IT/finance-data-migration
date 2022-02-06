using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.DmRunStatus;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.DmRunStatus;
using Microsoft.EntityFrameworkCore;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FinanceDataMigrationApi
{
    public class Handler
    {

        //private readonly IExtractTransactionEntityUseCase _extractTransactionsUseCase;
        //private readonly ITransformTransactionEntityUseCase _transformTransactionsUseCase;
        //private readonly ILoadTransactionEntityUseCase _loadTransactionsUseCase;

        readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        readonly IGetLastHintUseCase _getLastHintUseCase;
        readonly ITenureGetAllUseCase _tenureGetAllUseCase;
        readonly ITenureSaveToSqlUseCase _tenureSaveToSqlUseCase;
        readonly IAssetGetAllUseCase _assetGetAllUseCase;
        readonly IAssetSaveToSqlUseCase _assetSaveToSqlUseCase;
        readonly IDmRunStatusGetUseCase _dmRunStatusGetUseCase;
        readonly IDmAssetRunStatusSaveUseCase _dmAssetRunStatusSaveUseCase;
        readonly IDmTenureRunStatusSaveUseCase _dmTenureRunStatusSaveUseCase;
        readonly IDmChargeExtractRunStatusSaveUseCase _dmChargeExtractRunStatusSaveUseCase;
        readonly IDmChargeLoadRunStatusSaveUseCase _dmChargeLoadRunStatusSaveUseCase;
        readonly ITimeLogSaveUseCase _timeLogSaveUseCase;
        /// <summary>
        /// Waiting time for next run, in second
        /// </summary>
        private readonly int _waitDuration;

        private readonly int _batchSize;

        public Handler()
        {
            _waitDuration = int.Parse(Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "5");
            _batchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "100");

            DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (connectionString != null)
                optionsBuilder.UseSqlServer(connectionString);
            else
                throw new Exception($"Connection string is null.");

            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);


            IDMRunLogGateway migrationRunGateway = new DMRunLogGateway(context);

            #region Commented
            /*IDMTransactionEntityGateway dMTransactionEntityGateway = new DMTransactionEntityGateway(context);
            var httpClient = new HttpClient();
            ITransactionGateway transactionGateway = new TransactionGateway(httpClient);

            IGetEnvironmentVariables getEnvironmentVariables = new GetEnvironmentVariables();
            ICustomeHttpClient _customeHttpClient = new CustomeHttpClient();
            ITenureGateway tenureGateway = new TenureGateway(_customeHttpClient, getEnvironmentVariables);

            _extractTransactionsUseCase = new ExtractTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway);

            _transformTransactionsUseCase = new TransformTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway, tenureGateway);

            _loadTransactionsUseCase = new LoadTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway, transactionGateway);*/
            #endregion

            /*var url = Environment.GetEnvironmentVariable("DynamoDb_LocalServiceUrl");
            var clientConfig = new AmazonDynamoDBConfig { ServiceURL = url };*/

            IAmazonDynamoDB amazonDynamoDb = new AmazonDynamoDBClient();
            IDynamoDBContext dynamoDbContext = new DynamoDBContext(amazonDynamoDb);
            IChargeGateway chargeGateway = new ChargeGateway(context, amazonDynamoDb);
            ITenureGateway tenureGateway = new TenureGateway(context, amazonDynamoDb, dynamoDbContext);
            IAssetGateway assetGateway = new AssetGateway(context, amazonDynamoDb);
            IHitsGateway hitsGateway = new HitsGateway(context);
            IDmRunStatusGateway dmRunStatusGateway = new DmRunStatusGateway(context);
            ITimeLogGateway timeLogGateway = new TimeLogGateway(context);

            _getLastHintUseCase = new GetLastHintUseCase(hitsGateway);
            _loadChargeEntityUseCase = new LoadChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _extractChargeEntityUseCase = new ExtractChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _tenureGetAllUseCase = new TenureGetAllUseCase(tenureGateway);
            _tenureSaveToSqlUseCase = new TenureSaveToSqlUseCase(tenureGateway);
            _assetGetAllUseCase = new AssetGetAllUseCase(assetGateway);
            _assetSaveToSqlUseCase = new AssetSaveToSqlUseCase(assetGateway);
            _dmRunStatusGetUseCase = new DmRunStatusGetUseCase(dmRunStatusGateway);
            _dmAssetRunStatusSaveUseCase = new DmAssetRunStatusSaveUseCase(dmRunStatusGateway);
            _dmTenureRunStatusSaveUseCase = new DmTenureRunStatusSaveUseCase(dmRunStatusGateway);
            _dmChargeExtractRunStatusSaveUseCase = new DmChargeExtractRunStatusSaveUseCase(dmRunStatusGateway);
            _dmChargeLoadRunStatusSaveUseCase = new DmChargeLoadRunStatusSaveUseCase(dmRunStatusGateway);
            _timeLogSaveUseCase = new TimeLogSaveUseCase(timeLogGateway);
        }

        /*public async Task<StepResponse> ExtractTransactions()
        {
            return await _extractTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> TransformTransactions()
        {
            return await _transformTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> LoadTransactions()
        {
            return await _loadTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }*/

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
                        await _dmChargeLoadRunStatusSaveUseCase.ExecuteAsync(DateTime.Today).ConfigureAwait(false);

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
                    await _dmChargeExtractRunStatusSaveUseCase.ExecuteAsync(DateTime.Today).ConfigureAwait(false);
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

        public async Task<StepResponse> DownloadTenureToIfs()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("TENURE_DOWNLOAD_BATCH_SIZE") ??
                                      throw new Exception("Tenure download batch size is null."));

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
                var response = await _tenureGetAllUseCase.ExecuteAsync(count, lastEvaluatedKey).ConfigureAwait(false);
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

        public async Task<StepResponse> DownloadAssetToIfs()
        {
            try
            {
                int count = int.Parse(Environment.GetEnvironmentVariable("ASSET_DOWNLOAD_BATCH_SIZE") ??
                                      throw new Exception("Asset download batch size is null."));

                var dmRunStatus = await _dmRunStatusGetUseCase.ExecuteAsync().ConfigureAwait(false);

                if (dmRunStatus.AllAssetDmCompleted == true)
                    return new StepResponse() { Continue = false };

                var lastHint = await _getLastHintUseCase.ExecuteAsync("asset").ConfigureAwait(false);

                DmTimeLogModel dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_assetGetAllUseCase)}",
                    StartTime = DateTime.Now
                };

                var response = await _assetGetAllUseCase.ExecuteAsync(count, lastHint == Guid.Empty ? "" : lastHint.ToString()).ConfigureAwait(false);

                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                if (response.lastHitId == null || response.Results.Assets.Count == 0)
                {
                    await _dmAssetRunStatusSaveUseCase.ExecuteAsync(true).ConfigureAwait(false);
                    return new StepResponse() { Continue = false };
                }

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"Assets.ToXElement",
                    StartTime = DateTime.Now
                };
                var xmlData = response.Results.Assets.ToXElement();
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

                lastHint = Guid.Parse(response.lastHitId);

                dmTimeLogModel = new DmTimeLogModel()
                {
                    ProcName = $"{nameof(_assetSaveToSqlUseCase)}",
                    StartTime = DateTime.Now
                };
                await _assetSaveToSqlUseCase.ExecuteAsync(lastHint.ToString(), xmlData).ConfigureAwait(false);
                await _timeLogSaveUseCase.ExecuteAsync(dmTimeLogModel).ConfigureAwait(false);

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
    }
}
