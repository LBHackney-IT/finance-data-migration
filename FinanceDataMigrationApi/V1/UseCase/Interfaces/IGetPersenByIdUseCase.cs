using System;
using System.Threading.Tasks;
using Hackney.Shared.Person;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetPersenByIdUseCase
    {
        public Task<Person> ExecuteAsync(Guid id);
    }
}
