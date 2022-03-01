using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class LoadTransactionEntityUseCase : ILoadTransactionEntityUseCase
    {
        readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        readonly int _batchSize = 25;
        readonly ITransactionGateway _transactionGateway;

        private const string DataMigrationTask = "LOAD";

        public LoadTransactionEntityUseCase(ITransactionGateway transactionGateway)
        {
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                var extractedList = await _transactionGateway.GetExtractedListAsync(count).ConfigureAwait(false);
                if (extractedList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i <= extractedList.Count / _batchSize; i++)
                    {
                        var data = extractedList.OrderBy(p => p.Id).Skip(i * _batchSize).Take(_batchSize).ToList();
                        if (data.Any())
                        {
                            tasks.Add(_transactionGateway.BatchInsert(data));
                            await Task.WhenAll(tasks).ConfigureAwait(false);
                            tasks.Clear();
                        }
                    }
                    if (tasks.Count > 0)
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity.");
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
                                        $" load transaction exception: {ex.Message}");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
        }
    }
}
