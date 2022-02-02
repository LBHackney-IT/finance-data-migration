using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TimelogGateway : ITimelogGateway
    {
        private readonly DatabaseContext _context;

        public TimelogGateway(DatabaseContext context)
        {
            _context = context;
        }
        public Task Save(TimeLogModel timeLogModel)
        {
                
        }
    }
}
