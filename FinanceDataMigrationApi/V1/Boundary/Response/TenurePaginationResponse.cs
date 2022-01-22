using System.Collections.Generic;
using Hackney.Shared.Tenure.Infrastructure;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class TenurePaginationResponse
    {
        //public Dictionary<string, AttributeValue> LastKey { get; set; }
        public string PaginationToken { get; set; }
        public List<TenureInformationDb> TenureInformations { get; set; }
    }
}
