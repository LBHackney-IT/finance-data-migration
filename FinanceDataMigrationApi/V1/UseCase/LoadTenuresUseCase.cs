using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class LoadTenuresUseCase : ILoadTenuresUseCase
    {
        private readonly ITenureAPIGateway _tenureGateway;
        private readonly IDMAccountEntityGateway _dMAccountEntityGateway;

        public LoadTenuresUseCase(ITenureAPIGateway tenureGateway,
            IDMAccountEntityGateway dMAccountEntityGateway)
        {
            _tenureGateway = tenureGateway;
            _dMAccountEntityGateway = dMAccountEntityGateway;
        }

        public async Task ExecuteAsync()
        {
            var prnList = new List<string>
            {
                "47211422",
                "18057818"
            };

            var tenures = await _tenureGateway.GetTenuresByPrnAsync(prnList).ConfigureAwait(false);

            var tenureEntities = tenures.Select(t => new DMTenureEntity
            {
                HouseholdMembers = JsonConvert.SerializeObject(t.HouseholdMembers),
                PaymentReference = t.PaymentReference,
                TenuredAsset = JsonConvert.SerializeObject(t.TenuredAsset),
                TenureType = JsonConvert.SerializeObject(t.TenureType)
            }).ToList();

            await _dMAccountEntityGateway.SaveTenureListAsync(tenureEntities).ConfigureAwait(false);
        }
    }
}
