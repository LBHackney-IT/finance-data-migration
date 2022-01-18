using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using Hackney.Shared.HousingSearch.Domain.Accounts;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class ResponseFactory
    {
        //public static MigrationRunResponse ToResponse(this MigrationRunDomain domain)
        //{
        //    return domain == null ? null : new MigrationRunResponse()
        //    {
        //        Id = domain.Id,
        //        DynamoDbEntity = domain.DynamoDbEntityName,
        //        LastRunStatus = domain.LastRunStatus,
        //        LastRunDate = domain.LastRunDate,
        //        ExpectedRowsToMigrate = domain.ExpectedRowsToMigrate,
        //        ActualRowsMigrated = domain.ActualRowsMigrated,
        //        StartRowId = domain.StartRowId,
        //        EndRowId = domain.EndRowId,
        //        UpdatedAt = domain.UpdatedAt
        //    };
        //}

        //public static List<MigrationRunResponse> ToResponse(this IEnumerable<MigrationRunDomain> domainList)
        //{
        //    return domainList == null ?
        //        new List<MigrationRunResponse>() :
        //        domainList.Select(domain => domain.ToResponse()).ToList();
        //}
    }
}
