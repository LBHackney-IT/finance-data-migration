using System;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public class DmDynamoLastHInt
    {
        public string TableName { get; set; }
        public Guid Id { get; set; }
        public DateTime Timex { get; set; }
    }
}
