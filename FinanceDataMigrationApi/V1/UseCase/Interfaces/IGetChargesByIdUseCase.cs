using FinanceDataMigrationApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetChargesByIdUseCase
    {
        Task<List<ChargeResponse>> ExecuteAsync(Guid targetId);
    }
}
