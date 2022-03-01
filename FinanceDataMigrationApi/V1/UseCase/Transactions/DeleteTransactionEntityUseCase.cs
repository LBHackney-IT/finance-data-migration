using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class DeleteTransactionEntityUseCase : IDeleteTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly ITransactionGateway _transactionGateway;

        readonly int _batchSize = 25;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TransactionDelete";

        public DeleteTransactionEntityUseCase(IDMRunLogGateway dMRunLogGateway, ITransactionGateway transactionGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                var forDeleteList = await _transactionGateway.GetToBeDeletedListForDeleteAsync(count).ConfigureAwait(false);
                if (forDeleteList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i <= forDeleteList.Count / _batchSize; i++)
                    {
                        var data = forDeleteList.OrderBy(p => p.Id).Skip(i * _batchSize).Take(_batchSize).ToList();
                        /*await _accountsGateway.BatchDelete(data).ConfigureAwait(false);*/
                        if (data.Any())
                        {
                            tasks.Add(_transactionGateway.BatchDelete(data));
                            if (tasks.Count == 5)
                            {
                                await Task.WhenAll(tasks).ConfigureAwait(false);
                                System.Threading.Thread.Sleep(4000);
                                tasks.Clear();
                            }
                        }
                    }
                    if (tasks.Count > 0)
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Accounts} Entity.");
                    return new StepResponse()
                    {
                        Continue = false
                    };
                }

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}" +
                                        $".{nameof(Handler)}" +
                                        $".{nameof(ExecuteAsync)}" +
                                        $" Delete account exception: {ex.Message}");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
        }
    }
}
