using FinanceDataMigrationApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges
{
    public interface IDeleteAllChargesEntityUseCase
    {
        Task<StepResponse> ExecuteAsync();
    }
}
