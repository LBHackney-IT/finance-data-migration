using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class PersonGateway : IPersonGateway
    {
        public Task<Person> GetPersonByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
