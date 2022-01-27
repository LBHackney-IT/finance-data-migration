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
            var dmRunLogDomain = await UpdateLoadInProcessStatusForDmRunLogDomain().ConfigureAwait(false);

            // Get all the Charges entity extracted data from the SOW2b SQL Server database table DMChargesEntity,
            //      where isTransformed flag is TRUE and isLoaded flag is FALSE
            var transformedList = await _dMChargeGateway.GetTransformedListAsync().ConfigureAwait(false);

            if (transformedList.Any())
            {

                //await _dynamoDbGateway.AddRangeAsync(addChargeRequestList).ConfigureAwait(false);

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < transformedList.Count / 25; i++)
                {

                    tasks.Add(_dMChargeGateway.BatchInsert(transformedList.Skip(i).Take(25).ToList()));
                }
                DateTime startDateTime = DateTime.Now;
                await Task.WhenAll(tasks).ConfigureAwait(false);


                /*await UpdateIsLoadedStatusForDmChargeEntities(transformedList).ConfigureAwait(false);

                UpdateSuccessStatusForDmRunLogDomain(dmRunLogDomain, transformedList);*/
            }

            if (!transformedList.Any())
            {
                LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Charges} Entity");
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.NothingToMigrate.ToString();
            }

            await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

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

        /*private static void UpdateSuccessStatusForDmRunLogDomain(DMRunLogDomain dmRunLogDomain, IList<Charge> transformedList)
        {
            // Update migrationrun item with SET start_row_id & end_row_id here.
            //      and set status to "LoadCompleted" (Data Set Migrated successfully)
            dmRunLogDomain.ActualRowsMigrated = transformedList.Count;
            dmRunLogDomain.StartRowId = transformedList.First().Id;
            dmRunLogDomain.EndRowId = transformedList.Last().Id;
            dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadCompleted.ToString();
        }

        private async Task UpdateIsLoadedStatusForDmChargeEntities(IList<Charge> transformedList)
        {
            foreach (var dmChargeEntityDomain in transformedList)
            {
                dmChargeEntityDomain.MigrationStatus = EMigrationStatus.Loaded;
            }

            await _dMChargeGateway.UpdateDMChargeEntityItems(transformedList).ConfigureAwait(false);
        }*/
    }
}
