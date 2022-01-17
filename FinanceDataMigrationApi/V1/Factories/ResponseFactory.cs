using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;

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
        public static ChargeResponse ToResponse(this Charge domain)
        {
            return new ChargeResponse()
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                DetailedCharges = domain.DetailedCharges,
                TargetType = domain.TargetType
            };
        }
        public static List<ChargeResponse> ToResponse(this IEnumerable<Charge> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
