using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset;

namespace FinanceDataMigrationApi.V1.UseCase.Asset
{
    public class GetAllAssetsBySegmentScan: IGetAllAssetsBySegmentScan
    {
        private readonly IAssetGateway _gateway;

        public GetAllAssetsBySegmentScan(IAssetGateway gateway)
        {
            _gateway = gateway;
        }
        public async Task<List<Hackney.Shared.Asset.Domain.Asset>> ExecuteAsync()
        {
            return await _gateway.GetAllBySegmentScan().ConfigureAwait(false);

        }
    }
}
