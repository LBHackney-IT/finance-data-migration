using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class UpdateUseCase : IUpdateUseCase
    {
        private readonly IMigrationRunDynamoGateway _gateway;

        public UpdateUseCase(IMigrationRunDynamoGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<MigrationRunResponse> ExecuteAsync(MigrationRunUpdateRequest migrationRun, Guid id)
        {
            var migrationRunDomain = migrationRun.ToDomain();

            migrationRunDomain.Id = id;
            migrationRunDomain.DynamoDbEntity = migrationRun.DynamoDbEntity;
            migrationRunDomain.StartRowId = migrationRun.StartRowId;
            migrationRunDomain.EndRowId = migrationRun.EndRowId;
            migrationRunDomain.ActualRowsMigrated = migrationRun.ActualRowsMigrated;
            migrationRunDomain.ExpectedRowsToMigrate = migrationRun.ExpectedRowsToMigrate;
            migrationRunDomain.LastRunStatus = migrationRun.LastRunStatus;
            migrationRunDomain.LastRunDate = migrationRun.LastRunDate;

            await _gateway.UpdateAsync(migrationRunDomain).ConfigureAwait(false);

            return migrationRunDomain.ToResponse();
        }
    }
}
