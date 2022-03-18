using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class BatchIndexTransactionEntityUseCase : Interfaces.Transactions.IBatchIndexTransactionEntityUseCase
    {
        private readonly ITransactionGateway _transactionGateway;
        private readonly IEsGateway<QueryableTransaction> _esGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "2";

        public BatchIndexTransactionEntityUseCase(ITransactionGateway dMTransactionEntityGateway, IEsGateway<QueryableTransaction> esGateway)
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
            var loadedList = await _transactionGateway.GetLoadedListAsync(count).ConfigureAwait(false);
            var transactionRequestList = loadedList.ToTransactionRequestList();
            var esRequests = EsFactory.ToTransactionRequestList(transactionRequestList);
            List<Task> tasks = new List<Task>();

            for (int i = 0; i <= esRequests.Count / 500; i++)
            {
                tasks.Add(_esGateway.BulkIndex(esRequests.Skip(i * 1000).Take(1000)));
            }

            if (tasks.Any())
                await Task.WhenAll(tasks).ConfigureAwait(false);

            return new StepResponse { Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration)) };
        }
    }
}
