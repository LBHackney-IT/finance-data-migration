using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class LoadChargeEntityUseCase : ILoadChargeEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IChargeGateway _dMChargeGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "LOAD";

        public LoadChargeEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IChargeGateway dMChargeGateway
        )
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMChargeGateway = dMChargeGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            var transformedList = await _dMChargeGateway.GetTransformedListAsync().ConfigureAwait(false);

            if (transformedList.Any())
            {

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < transformedList.Count / 25; i++)
                {
                    /*await _dMChargeGateway.BatchInsert(transformedList.Skip(i * 25).Take(25).ToList())
                        .ConfigureAwait(false)*/
                    ;
                    tasks.Add(_dMChargeGateway.BatchInsert(transformedList.Skip(i * 25).Take(25).ToList()));
                }
                DateTime startDateTime = DateTime.Now;
                await Task.WhenAll(tasks).ConfigureAwait(false);

            }

            if (!transformedList.Any())
            {
                LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Charges} Entity");
            }

            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };

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
