using System.Text.Json.Serialization;

namespace FinanceDataMigrationApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
        LoadCompleted,
        LoadFailed,
        NothingToMigrate,
        IndexInprogress,
        IndexCompleted,
        IndexFailed
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeType
    {
        Estate,
        Block,
        Property
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeGroup
    {
        Tenants,
        Leaseholders
    }
}
