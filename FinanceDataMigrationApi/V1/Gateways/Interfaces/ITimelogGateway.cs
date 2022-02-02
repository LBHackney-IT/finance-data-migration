using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    interface ITimelogGateway
    {
        public Task Save(TimeLogModel timeLogModel);
    }
}
