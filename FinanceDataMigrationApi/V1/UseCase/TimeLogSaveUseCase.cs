using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class TimeLogSaveUseCase : ITimeLogSaveUseCase
    {
        private readonly ITimeLogGateway _gateway;

        public TimeLogSaveUseCase(ITimeLogGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task ExecuteAsync(DmTimeLogModel timeLogModel)
        {
            timeLogModel.EndTime = DateTime.Now;
            timeLogModel.ElapsedTime = DateTime.Now.Subtract(timeLogModel.EndTime);
            await _gateway.Save(timeLogModel).ConfigureAwait(false);
        }
    }
}
