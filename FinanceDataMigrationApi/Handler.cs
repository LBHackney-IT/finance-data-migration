using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FinanceDataMigrationApi
{
    public class Handler
    {

        //private readonly IExtractTransactionEntityUseCase _extractTransactionsUseCase;
        //private readonly ITransformTransactionEntityUseCase _transformTransactionsUseCase;
        //private readonly ILoadTransactionEntityUseCase _loadTransactionsUseCase;

        public Handler()
        {
        //    DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
        //    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        //    optionsBuilder.UseSqlServer(connectionString);
        //    DatabaseContext context = new DatabaseContext(optionsBuilder.Options);

        //    IDMRunLogGateway migrationRunGateway = new DMRunLogGateway(context);
        //    IDMTransactionEntityGateway dMTransactionEntityGateway = new DMTransactionEntityGateway(context);
        //    var httpClient = new HttpClient();
        //    ITransactionGateway transactionGateway = new TransactionGateway(httpClient);

        //    IGetEnvironmentVariables getEnvironmentVariables = new GetEnvironmentVariables();
        //    ICustomeHttpClient _customeHttpClient = new CustomeHttpClient();
        //    ITenureGateway tenureGateway = new TenureGateway(_customeHttpClient, getEnvironmentVariables);

        //    _extractTransactionsUseCase = new ExtractTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway);

        //    _transformTransactionsUseCase = new TransformTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway, tenureGateway);

        //    _loadTransactionsUseCase = new LoadTransactionEntityUseCase(migrationRunGateway, dMTransactionEntityGateway, transactionGateway);
        //}

        //public async Task<StepResponse> ExtractTransactions()
        //{
        //    return await _extractTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        //}

        //public async Task<StepResponse> TransformTransactions()
        //{
        //    return await _transformTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        //}

        //public async Task<StepResponse> LoadTransactions()
        //{
        //    return await _loadTransactionsUseCase.ExecuteAsync().ConfigureAwait(false);
        }

    }

}
