using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class DmRunStatusGateway : IDmRunStatusGateway
    {
        private DatabaseContext _context;

        public DmRunStatusGateway(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<DmRunStatusModel> GetData()
        {
            _context = DatabaseContext.Create();
            return await _context.DmRunStatusModels.FirstAsync().ConfigureAwait(false);
        }

        public async Task SaveStatus(DmRunStatusModel model)
        {
            var item = _context.DmRunStatusModels.FirstOrDefault(p => p.Id == 1);
            if (item == null)
                throw new Exception("DmRunStatus is empty.");

            item.AllAssetDmCompleted = model.AllAssetDmCompleted;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
