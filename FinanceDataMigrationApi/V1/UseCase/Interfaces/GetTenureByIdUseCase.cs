using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public class GetTenureByIdUseCase : IGetByIdUseCase
    {
        private readonly ITenureGateway _gateway;



        public GetTenureByIdUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }

        Task<TenureInformation> IGetByIdUseCase.Execute(TenureQueryRequest query)
        {
            throw new System.NotImplementedException();
        }
    }
}
