using System.Collections.Generic;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class TenureResponse
    {
        public long Total { get; set; }
        public List<TenureInformation> Tenures { get; set; }
    }
}
