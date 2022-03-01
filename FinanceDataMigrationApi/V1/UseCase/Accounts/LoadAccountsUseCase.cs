using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class LoadAccountsUseCase : ILoadAccountsUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IAccountsGateway _accountsGateway;

        readonly int _batchSize = 25;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "AccountLoad";

        public LoadAccountsUseCase(IDMRunLogGateway dMRunLogGateway, IAccountsGateway accountsGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _accountsGateway = accountsGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                var extractedList = await _accountsGateway.GetExtractedListAsync(count).ConfigureAwait(false);
                /*var extractedList = await _accountsGateway.GetLoadFailedListAsync(count).ConfigureAwait(false);*/
                if (extractedList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i < extractedList.Count; i++)
                    {
                        var data = extractedList.OrderBy(p => p.Id).Skip(i * _batchSize).Take(_batchSize).ToList();
                        if (data.Any())
                        {
                            tasks.Add(_accountsGateway.BatchInsert(data));
                            await Task.WhenAll(tasks).ConfigureAwait(false);
                            tasks.Clear();
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
                                        $" load account exception: {ex.Message}");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
        }
    }
}
