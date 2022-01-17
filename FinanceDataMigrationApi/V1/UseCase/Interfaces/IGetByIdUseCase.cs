using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        Task<TenureInformation> Execute(TenureQueryRequest query);
    }
}
