using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace FinanceDataMigrationApi.V1.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, AttributeValue> PureAdd(this Dictionary<string, AttributeValue> me, string key, AttributeValue value)
        {
            if (value == null ||
                value.NULL || (
                    value.S == null &&
                    value.N == null &&
                    value.B == null &&
                    value.SS.Count == 0 &&
                    value.NS.Count == 0 &&
                    value.BS.Count == 0 &&
                    value.M.Count == 0 &&
                    value.L.Count == 0))
                return null;

            me.Add(key, value);
            return new Dictionary<string, AttributeValue>()
                {
                    {key,value}
                };

        }
    }
}
