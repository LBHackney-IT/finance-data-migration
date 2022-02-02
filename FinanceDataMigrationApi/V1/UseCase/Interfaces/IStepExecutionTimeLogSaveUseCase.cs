using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IStepExecutionTimelogSaveUseCase
    {
        Task ExecuteAsync(TimeLogModel timeLogModel);
    }
}
