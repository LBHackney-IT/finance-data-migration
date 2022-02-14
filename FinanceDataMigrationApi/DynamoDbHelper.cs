using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public static class DynamoDbHelper
    {

        public static async Task<bool> GetRecords<T>(IDynamoDBContext dynamoDBContext)
        {
            try
            {
                var table = dynamoDBContext.GetTargetTable<T>();
                var filter = new ScanFilter();
                string paginationToken = "{}";
                do
                {
                    var result = table.Scan(new ScanOperationConfig
                    {
                        Limit = 100,
                        PaginationToken = paginationToken,
                        Filter = filter
                    });
                    var items = await result.GetNextSetAsync().ConfigureAwait(false);

                    if (items.Count > 0)
                    {

                        IEnumerable<T> itemResults = dynamoDBContext.FromDocuments<T>(items);
                        await BatchDelete(itemResults, dynamoDBContext).ConfigureAwait(false);
                        paginationToken = result.PaginationToken;
                    }
                }
                while (!string.Equals(paginationToken, "{}", StringComparison.Ordinal));

                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        public static async Task BatchDelete<T>(IEnumerable<T> documents, IDynamoDBContext dynamoDBContext)
        {

            var batch = dynamoDBContext.CreateBatchWrite<T>();
            batch.AddDeleteItems(documents);
            await batch.ExecuteAsync().ConfigureAwait(false);
        }
    }
}
