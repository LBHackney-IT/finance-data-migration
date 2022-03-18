using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain.Assets;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset;

namespace FinanceDataMigrationApi.V1.UseCase.Asset
{
    public class GetAllAssetsBySegmentScan : IGetAllAssetsBySegmentScan
    {
        private readonly IAssetGateway _gateway;
        private readonly IEsGateway<QueryableAsset> _esGateway;

        public GetAllAssetsBySegmentScan(IAssetGateway gateway, IEsGateway<QueryableAsset> esGateway)
        {
            _gateway = gateway;
            _esGateway = esGateway;
        }
        public async Task<List<Hackney.Shared.Asset.Domain.Asset>> ExecuteAsync()
        {
            var response = await _gateway.GetAllBySegmentScan().ConfigureAwait(false);
            var maxBatchCount = 1000;

            int loopCount;
            if (response.Count % maxBatchCount == 0)
                loopCount = response.Count / maxBatchCount;
            else
                loopCount = (response.Count / maxBatchCount) + 1;

            for (var start = 0; start < loopCount; start++)
            {
                var itemsToIndex = response.Skip(start * maxBatchCount).Take(maxBatchCount);
                var esRequests = EsFactory.ToAssetRequestList(itemsToIndex);
                //await _esGateway.BulkIndex(esRequests).ConfigureAwait(false);
            }

            return response;

        }
    }
}
