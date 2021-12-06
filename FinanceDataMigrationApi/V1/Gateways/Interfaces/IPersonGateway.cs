using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    interface IPersonGateway
    {
        Task<Person> GetPersonByIdAsync(int id);

    }
}
