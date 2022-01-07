using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Asset;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAssetGateway
    {
        public Task<Asset> GetById(string id);
    }
}
