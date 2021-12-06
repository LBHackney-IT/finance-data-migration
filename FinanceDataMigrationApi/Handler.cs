using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using AutoMapper;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FinanceDataMigrationApi
{
    public class Handler
    {

        private readonly IExtractTransactionEntityUseCase _extractTransactionsUseCase;
        private readonly ITransformTransactionEntityUseCase _transformTransactionsUseCase;
        private readonly ILoadTransactionEntityUseCase _loadTransactionsUseCase;

        private readonly IMapper _autoMapper;

        public Handler(IMapper autoMapper)
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            var clientConfig = new AmazonDynamoDBConfig
            {
                // Set the endpoint URL
                ServiceURL = "http://localhost:8000"
            };
            var client = new AmazonDynamoDBClient(clientConfig);
            //var client = new AmazonDynamoDBClient();
            DynamoDBContext dynamoDbContext = new DynamoDBContext(client);

            IMigrationRunDynamoGateway migrationRunGateway = new DynamoDbGateway(dynamoDbContext);
            IDMTransactionEntityGateway dMTransactionEntityGateway = new DMTransactionEntityGateway(context);

            _autoMapper = autoMapper;

            _extractTransactionsUseCase = new ExtractTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway);

            //TODO
            _transformTransactionsUseCase = new TransformTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway);

            //TODO
            _loadTransactionsUseCase = new LoadTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway);

        }

        public async Task<StepResponse> ExtractTransactions()
        {
            return await _extractTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        //TODO
        public async Task<StepResponse> TransformTransactions()
        {
            return await _transformTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

        //TODO
        public async Task<StepResponse> LoadTransactions()
        {
            return await _loadTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

    }

}
