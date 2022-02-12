using System.Text.Json.Serialization;

namespace FinanceDataMigrationApi.V1.Infrastructure.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeSubGroup
    {
        Actual,
        Estimate
    }
}
