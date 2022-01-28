using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class TenurePaginationResponse
    {
        public Dictionary<string, AttributeValue> LastKey { get; set; }
        //public string PaginationToken { get; set; }
        public List<TenureInformation> TenureInformation { get; set; }
    }
}
