using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Tenure;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITenureBatchInsertUseCase
    {
        Task<bool> ExecuteAsync(List<TenureInformation> tenures);
    }
}
