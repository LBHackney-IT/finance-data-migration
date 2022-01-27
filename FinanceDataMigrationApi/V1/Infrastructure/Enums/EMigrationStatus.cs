using EFCore.BulkExtensions.SqlAdapters;

namespace FinanceDataMigrationApi.V1.Infrastructure.Enums
{
    public enum EMigrationStatus
    {
        Extracted,Transformed,Loading,Loaded,LoadFailed
    }
}
