using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAssetGateway
    {

        public Task<APIResponse<GetAssetListResponse>> DownloadAsync(string lastHintStr);
        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml);
        public Task<Guid> GetLastHint();
    }
}
