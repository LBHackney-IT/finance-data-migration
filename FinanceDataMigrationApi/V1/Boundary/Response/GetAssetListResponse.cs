using System.Collections.Generic;
using Hackney.Shared.HousingSearch.Domain.Asset;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class GetAssetListResponse
    {
        private long _total;

        public List<Asset> Assets { get; set; }

        public GetAssetListResponse()
        {
            Assets = new List<Asset>();
        }

        public void SetTotal(long total)
        {
            _total = total < 0 ? 0 : total;
        }

        public long Total()
        {
            return _total;
        }
    }
}
