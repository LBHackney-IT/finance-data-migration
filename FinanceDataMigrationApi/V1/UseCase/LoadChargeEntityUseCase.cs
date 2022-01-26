using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class LoadChargeEntityUseCase : ILoadChargeEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMChargeEntityGateway _dMChargeEntityGateway;
        private readonly DynamoDbGateway _dynamoDbGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "LOAD";

        public LoadChargeEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IDMChargeEntityGateway dMChargeEntityGateway,
            DynamoDbGateway dynamoDbGateway
        )
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMChargeEntityGateway = dMChargeEntityGateway;
            _dynamoDbGateway = dynamoDbGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            var dmRunLogDomain = await UpdateLoadInProcessStatusForDmRunLogDomain().ConfigureAwait(false);

            // Get all the Charges entity extracted data from the SOW2b SQL Server database table DMChargesEntity,
            //      where isTransformed flag is TRUE and isLoaded flag is FALSE
            var transformedList = await _dMChargeEntityGateway.GetTransformedListAsync().ConfigureAwait(false);

            // for each row from the Transformed List make into a charge
            var addChargeRequestList = transformedList.ToAddChargeRequestList();

            if (addChargeRequestList.Any())
            {
                await _dynamoDbGateway.AddRangeAsync(addChargeRequestList).ConfigureAwait(false);

                await UpdateIsLoadedStatusForDmChargeEntities(transformedList).ConfigureAwait(false);

                UpdateSuccessStatusForDmRunLogDomain(dmRunLogDomain, transformedList);
            }

            if (!addChargeRequestList.Any())
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

        private static void UpdateSuccessStatusForDmRunLogDomain(DMRunLogDomain dmRunLogDomain, IList<DMChargeEntityDomain> transformedList)
        {
            // Update migrationrun item with SET start_row_id & end_row_id here.
            //      and set status to "LoadCompleted" (Data Set Migrated successfully)
            dmRunLogDomain.ActualRowsMigrated = transformedList.Count;
            dmRunLogDomain.StartRowId = transformedList.First().Id;
            dmRunLogDomain.EndRowId = transformedList.Last().Id;
            dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadCompleted.ToString();
        }

        private async Task UpdateIsLoadedStatusForDmChargeEntities(IList<DMChargeEntityDomain> transformedList)
        {
            foreach (var dmChargeEntityDomain in transformedList)
            {
                dmChargeEntityDomain.IsLoaded = true;
            }

            await _dMChargeEntityGateway.UpdateDMChargeEntityItems(transformedList).ConfigureAwait(false);
        }
    }
}