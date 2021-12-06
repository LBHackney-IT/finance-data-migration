using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public interface IMigrationRunDynamoGateway
    {
        public Task AddAsync(MigrationRun migrationRun);

        public Task<MigrationRun> GetMigrationRunByIdAsync(Guid id);

        public Task<MigrationRun> GetMigrationRunByEntityNameAsync(string entityName);

        public Task UpdateAsync(MigrationRun migrationRun);


        //public Task<UpdateEntityResult<MigrationRunDbEntity>> UpdateAsync(MigrationRunUpdateRequest requestObject, string requestBody, Guid id);

        public Task<List<MigrationRun>> GetAllMigrationRunsAsync();
    }
}
