namespace FinanceDataMigrationApi.V1.Boundary.Response.MetaData
{
    public class APIResponse<T> where T : class
    {
        public T Results { get; set; }

        public long Total { get; set; }

        public APIResponse() { }

        public APIResponse(T result)
        {
            Results = result;
        }
    }
}
