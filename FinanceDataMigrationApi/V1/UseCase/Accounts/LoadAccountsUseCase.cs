using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class LoadAccountsUseCase : ILoadAccountsUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMAccountEntityGateway _dMAccountEntityGateway;
        private readonly IEsGateway _esGateway;
        private readonly IAccountsDynamoDbGateway _accountsDynamoDbGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "LOAD";

        public LoadAccountsUseCase(IDMRunLogGateway dMRunLogGateway,
            IEsGateway esGateway,
            IDMAccountEntityGateway dMAccountEntityGateway,
            IAccountsDynamoDbGateway accountsDynamoDbGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _esGateway = esGateway;
            _dMAccountEntityGateway = dMAccountEntityGateway;
            _accountsDynamoDbGateway = accountsDynamoDbGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                var transformedList = (await _dMAccountEntityGateway.GetTransformedListAsync().ConfigureAwait(false)).ToList();

                LoggingHandler.LogInfo($"Start batch insert for {DMEntityNames.Accounts} entity");

                // ToDO: what if some batch  operation was failes?
                List<Task> tasks = new List<Task>();
                for (int i = 0; i <= transformedList.Count / 25; i++)
                {
                    var accountsToInsert = transformedList.Skip(i * 25).Take(25).ToList();

                    tasks.Add(_accountsDynamoDbGateway.BatchInsert(accountsToInsert));
                }

                DateTime startDateTime = DateTime.Now;
                await Task.WhenAll(tasks).ConfigureAwait(false);
                LoggingHandler.LogInfo($"Batch insert elapsed time: {DateTime.Now.Subtract(startDateTime).TotalSeconds}");

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} Entity");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(LoadAccountsUseCase)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(ex.ToString());

                throw;
            }
        }
    }
}
