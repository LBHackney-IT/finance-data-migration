using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class LoadTenuresUseCase : ILoadTenuresUseCase
    {
        private readonly ITenureAPIGateway _tenureGateway;

        public LoadTenuresUseCase(ITenureAPIGateway tenureGateway)
        {
            _tenureGateway = tenureGateway;
        }

        public async Task ExecuteAsync()
        {
            var prnList = new List<string>
            {
                "47211422",
                "18057818"
            };

            var tenures = await _tenureGateway.GetTenuresByPrnAsync(prnList).ConfigureAwait(false);
        }
    }
}
