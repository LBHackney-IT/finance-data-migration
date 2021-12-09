using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FinanceDataMigrationApi
{
    public static class EnumExtensions
    {
        public static TransactionType TransactionTypeEnumValue(this string stringValue)
        {
            try
            {
                return (TransactionType) Enum.Parse(typeof(TransactionType), stringValue);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{typeof(TransactionType)} : {e.Message}");
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        public static TargetType TargetTypeEnumValue(this string stringValue)
        {
            try
            {
                return (TargetType) Enum.Parse(typeof(TargetType), stringValue);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{typeof(TargetType)} : {e.Message}");
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get human-readable version of enum.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>effective DisplayAttribute.Name of given enum</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>();

            var displayName = displayAttribute?.GetName();
            return displayName ?? enumValue.ToString();
        }

    }
}
