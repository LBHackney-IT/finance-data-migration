using System;
using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class LoadChargeEntityUseCase : ILoadChargeEntityUseCase
    {
        readonly int _batchSize = 25;// Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "25");
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IChargeGateway _dMChargeGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "LOAD";

        public LoadChargeEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IChargeGateway dMChargeGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMChargeGateway = dMChargeGateway;
        }

        public async Task<StepResponse> ExecuteAsync(int count)
        {
            try
            {
                LoggingHandler.LogInfo($"charge load");
                var timer = new Stopwatch();
                timer.Start();
                var transformedList = await _dMChargeGateway.GetTransformedListAsync(count).ConfigureAwait(false);
                timer.Stop();
                //timer.Elapsed;
                if (transformedList.Any())
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i < transformedList.Count / _batchSize; i++)
                    {
                        tasks.Add(_dMChargeGateway.BatchInsert(transformedList.OrderBy(P => P.Id).Skip(i * _batchSize).Take(_batchSize).ToList()));
                    }
                    DateTime startDateTime = DateTime.Now;
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                if (!transformedList.Any())
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Charges} Entity");
                }

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Charges} Entity");

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
                    $" load charge exception: {ex.Message}");
                throw;
            }
        }

        private async Task<DMRunLogDomain> UpdateLoadInProcessStatusForDmRunLogDomain()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Charges} entity");

            // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "TransformCompleted"
            var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Charges)
                .ConfigureAwait(false);

            //Update migrationrun item with set status to "LoadInprogress".
            dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadInprogress.ToString();
            await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);
            return dmRunLogDomain;
        }
    }
}
