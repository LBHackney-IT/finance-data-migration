using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
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
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseSqlServer(connectionString);
            DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

            IMigrationRunGateway migrationRunGateway = new MigrationRunGateway(context);
            IDMTransactionEntityGateway dMTransactionEntityGateway = new DMTransactionEntityGateway(context);
            var httpClient = new HttpClient(); 
            ITransactionGateway transactionGateway = new TransactionGateway(httpClient);
            _autoMapper = autoMapper;

            _extractTransactionsUseCase = new ExtractTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway);

            _transformTransactionsUseCase = new TransformTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway);

            _loadTransactionsUseCase = new LoadTransactionEntityUseCase(_autoMapper, migrationRunGateway, dMTransactionEntityGateway, transactionGateway);

        }

        public async Task<StepResponse> ExtractTransactions()
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
        }

    }

}
