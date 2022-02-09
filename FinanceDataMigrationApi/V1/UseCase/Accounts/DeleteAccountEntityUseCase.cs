using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class DeleteAccountEntityUseCase : IDeleteAccountEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IAccountsGateway _accountsGateway;

        readonly int _batchSize = 25;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "AccountDelete";

        public DeleteAccountEntityUseCase(IDMRunLogGateway dMRunLogGateway, IAccountsGateway accountsGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _accountsGateway = accountsGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                var forDeleteList = await _accountsGateway.GetToBeDeletedListForDeleteAsync(count).ConfigureAwait(false);
                if (forDeleteList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i <= forDeleteList.Count / _batchSize; i++)
                    {
                        var data = forDeleteList.OrderBy(p => p.Id).Skip(i * _batchSize).Take(_batchSize).ToList();
                        /*await _accountsGateway.BatchDelete(data).ConfigureAwait(false);*/
                        if (data.Any())
                        {
                            tasks.Add(_accountsGateway.BatchDelete(data));
                            if (tasks.Count == 5)
                            {
                                await Task.WhenAll(tasks).ConfigureAwait(false);
                                System.Threading.Thread.Sleep(2000);
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
