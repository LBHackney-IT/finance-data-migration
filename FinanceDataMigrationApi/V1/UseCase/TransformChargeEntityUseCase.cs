using System;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class TransformChargeEntityUseCase : ITransformChargeEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IChargeGateway _dMChargeGateway;
        private readonly IAssetGateway _assetGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformChargeEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IChargeGateway dMChargeGateway,
            IAssetGateway assetGateway
            )
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMChargeGateway = dMChargeGateway;
            _assetGateway = assetGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Charges} entity");

            // Get latest migrationrun item from Table MigrationRuns with status ExtractCompleted, where is_feature_enabled flag is TRUE.
            var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Charges)
                .ConfigureAwait(false);

            // If there are rows to transform THEN
            if (dmRunLogDomain.ExpectedRowsToMigrate > 0)
            {
                // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Get all the Charge entity extracted data from the SOW2b SQL Server database table DMChargeEntity,
                //      where isTransformed flag is FALSE and isLoaded flag is FALSE
                var dMCharges = await _dMChargeGateway.ListAsync().ConfigureAwait(false);

                // Iterate through each row (or batched) and enrich with missing information for subsets
                foreach (var charge in dMCharges)
                {
                    //charge.TargetId = await GetAssetTargetId(charge.PropertyReference).ConfigureAwait(false);
                    var detailedCharge = await _dMChargeGateway
                        .GetDetailChargesListAsync(charge.PaymentReference).ConfigureAwait(false);
                    charge.DetailedCharges = JsonSerializer.Serialize(detailedCharge);
                    charge.IsTransformed = true;

                }


                // Update batched rows to staging table DMTChargeEntity.
                await _dMChargeGateway.UpdateDMChargeEntityItems(dMCharges).ConfigureAwait(false);

                // Update migrationrun item with set status to "TransformCompleted"
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformCompleted.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

            }

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }
    }
}
