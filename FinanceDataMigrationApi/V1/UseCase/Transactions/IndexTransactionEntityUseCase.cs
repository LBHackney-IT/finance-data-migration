using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class IndexTransactionEntityUseCase : IIndexTransactionEntityUseCase
    {
        private readonly ITransactionGateway _transactionGateway;
        private readonly IEsGateway<QueryableTransaction> _esGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "15";
        private const string DataMigrationTask = "INDEXING";

        public IndexTransactionEntityUseCase(ITransactionGateway dMTransactionEntityGateway, IEsGateway<QueryableTransaction> esGateway)
        {
            _transactionGateway = dMTransactionEntityGateway;
            _esGateway = esGateway;
        }
        /// <summary>
        /// Bulk index all loaded transactions into elastic search.
        /// </summary>
        /// <param name="count">batch size</param>
        /// <returns>Next execution statements</returns>
        public async Task<StepResponse> ExecuteAsync(int count)
        {
            DatabaseContext context = DatabaseContext.Create();
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            var loadedList = await _transactionGateway.GetLoadedListAsync(count).ConfigureAwait(false);

            var transactionRequestList = loadedList.ToTransactionRequestList();

            if (transactionRequestList.Any())
            {
                var esRequests = EsFactory.ToTransactionRequestList(transactionRequestList);
                await _esGateway.BulkIndex(esRequests).ConfigureAwait(false);
                context.TransactionEntities.Where(p =>
                        loadedList.Select(i => i.Id).Contains(p.Id))
                    .ForAll(p => p.MigrationStatus = EMigrationStatus.Indexed);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity");
                return new StepResponse()
                {
                    Continue = false
                };
            }

            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }
    }
}
