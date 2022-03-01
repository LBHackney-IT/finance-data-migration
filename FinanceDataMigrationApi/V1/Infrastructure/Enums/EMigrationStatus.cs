namespace FinanceDataMigrationApi.V1.Infrastructure.Enums
{
    public enum EMigrationStatus
    {
        Extracted, Transformed, Loading, Loaded, LoadFailed, Deleting, Deleted, DeleteFailed, ToBeDeleted
    }
}
