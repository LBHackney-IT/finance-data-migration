using System;

namespace FinanceDataMigrationApi.V1
{
    public static class Constants
    {
        public const string CorrelationId = "x-correlation-id";
        public const int LoadCount = 100;
        public static readonly int BatchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "25");
    }

}
