using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Handlers;

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
        readonly ITenureBatchInsertUseCase _tenureBatchInsertUseCase;
        readonly IGetLastHintUseCase _getLastHintUseCase;
        readonly ITenureGetAllUseCase _tenureGetAllUseCase;
        readonly ITenureSaveToSqlUseCase _tenureSaveToSqlUseCase;
        /// <summary>
        /// Waiting time for next run, in second
        /// </summary>
        private readonly int _waitDuration;

        private readonly int _batchSize;

        public Handler()
        {
            _waitDuration = int.Parse(Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "5");
            _batchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "100");

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
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
            IHitsGateway hitsGateway = new HitsGateway(context);

            _getLastHintUseCase = new GetLastHintUseCase(hitsGateway);
            _loadChargeEntityUseCase = new LoadChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _extractChargeEntityUseCase = new ExtractChargeEntityUseCase(migrationRunGateway, chargeGateway);
            _tenureBatchInsertUseCase = new TenureBatchInsertUseCase(tenureGateway);
            _tenureGetAllUseCase = new TenureGetAllUseCase(tenureGateway);
            _tenureSaveToSqlUseCase = new TenureSaveToSqlUseCase(tenureGateway);
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
            return await _loadChargeEntityUseCase.ExecuteAsync(100).ConfigureAwait(false);
        }

        public async Task<StepResponse> ExtractCharge()
        {
            return await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        public async Task<StepResponse> DownloadTenureToIfs(int count)
        {
            var lastKey = await _getLastHintUseCase.ExecuteAsync().ConfigureAwait(false);
            Dictionary<string, AttributeValue> lastEvaluatedKey = new Dictionary<string, AttributeValue>
                {
                    {"id",new AttributeValue{S = lastKey.ToString()}}
                };

            var response = await _tenureGetAllUseCase.ExecuteAsync(count, lastEvaluatedKey).ConfigureAwait(false);
            lastEvaluatedKey = response.LastKey;
            if (response.TenureInformation.Count == 0)
                return new StepResponse() { Continue = false };

            await _tenureSaveToSqlUseCase.ExecuteAsync(response.LastKey.Count > 0 ? lastEvaluatedKey["id"].S : lastKey.ToString(),
                response.TenureInformation.ToXElement()).ConfigureAwait(false);

            if (response.LastKey.Count == 0)
                return new StepResponse() { Continue = false };

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(_waitDuration)
            };
        }
    }
}
