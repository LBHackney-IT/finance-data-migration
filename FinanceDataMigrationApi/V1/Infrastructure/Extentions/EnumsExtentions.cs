using FinanceDataMigrationApi.V1.Handlers;
using System;

namespace FinanceDataMigrationApi.V1.Infrastructure.Extentions
{
    public static class EnumsExtentions
    {
        public static T ToEnumValue<T>(this string stringValue)
        {
            try
            {
                if(stringValue == null)
                {
                    return default(T);
                }

                return (T) Enum.Parse(typeof(T), stringValue);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{typeof(T)} : {e.Message}");
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }
    }
}
