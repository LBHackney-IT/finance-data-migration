using System.Text.Json.Serialization;

namespace FinanceDataMigrationApi.V1.Domain
{
    public enum MigrationRunStatus 
    {
        ReadyToMigrate,
        ExtractStarted,
        ExtractInprogress,
        ExtractCompleted,
        ExtractFailed,
        TransformInprogress,
        TransformCompleted,
        LoadInprogress,
        LoadCompleted
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        Tenure
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Rent,
        Charge
    }

}
