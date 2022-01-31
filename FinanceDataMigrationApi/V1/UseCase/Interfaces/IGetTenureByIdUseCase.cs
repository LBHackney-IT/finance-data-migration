using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface IGetTenureByIdUseCase
    {
        public Task<TenureInformation> ExecuteAsync(Guid id);
    }
}
