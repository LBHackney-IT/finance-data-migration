using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class Timelog : ITimelog
    {
        public Task Save(TimeLogModel timeLogModel)
        {
            throw new NotImplementedException();
        }
    }
}
