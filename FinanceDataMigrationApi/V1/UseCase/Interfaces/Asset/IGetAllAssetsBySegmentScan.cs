using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset
{
    public interface IGetAllAssetsBySegmentScan
    {
        public Task<List<Hackney.Shared.Asset.Domain.Asset>> ExecuteAsync();
    }
}
