using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllUseCase : IGetAllUseCase
    {
        //private readonly IExampleGateway _gateway;
        private readonly IMigrationRunDynamoGateway _gateway;
        public GetAllUseCase(IMigrationRunDynamoGateway gateway)
        {
            _gateway = gateway;
        }

        //[LogCall]
        public async Task<MigrationRunResponseList> ExecuteAsync()
        {
            MigrationRunResponseList migrationRunResponseList = new MigrationRunResponseList();
            List<MigrationRun> data = await _gateway.GetAllMigrationRunsAsync().ConfigureAwait(false);

            //return new MigrationRunResponseList { MigrationRunResponses = _gateway.GetAll().ToResponse() };
            return new MigrationRunResponseList { MigrationRunResponses = data.ToResponse() };
        }
    }
}
