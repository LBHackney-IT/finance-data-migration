namespace FinanceDataMigrationApi.V1.Infrastructure.Enums
{
    public enum EMigrationStatus
    {
        Extracted, Transformed, Loading, Loaded, LoadFailed, Indexing, Indexed, IndexingFailed
    }
}
