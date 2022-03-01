using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TimeLogGateway : ITimeLogGateway
    {
        private readonly DatabaseContext _context;

        public TimeLogGateway(DatabaseContext context)
        {
            _context = context;
        }
        public async Task Save(DmTimeLogModel timeLogModel)
        {
            _context.DmTimeLogModels.Add(timeLogModel);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
