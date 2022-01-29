using System;

namespace FinanceDataMigrationApi.V1
{
    public static class Constants
    {
        public const string CorrelationId = "x-correlation-id";
        public static readonly int LoadCount = Convert.ToInt32(Environment.GetEnvironmentVariable("LOAD_COUNT") ?? "100");
        public static readonly int BatchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "25");
    }

}
