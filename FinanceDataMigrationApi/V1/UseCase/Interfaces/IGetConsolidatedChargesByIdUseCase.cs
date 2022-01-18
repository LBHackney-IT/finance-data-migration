using Hackney.Shared.HousingSearch.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetConsolidatedChargesByIdUseCase
    {
        Task<List<ConsolidatedCharge>> ExecuteAsync(Guid targetId);
    }
}
