using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.Person;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IPersonGateway
    {
        public Task<Person> GetById(Guid id);
    }
}
