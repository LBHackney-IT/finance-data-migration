using FinanceDataMigrationApi.V1.Domain;
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
            return (TransactionType) Enum.Parse(typeof(TransactionType), stringValue);
        }

        public static TargetType TargetTypeEnumValue(this string stringValue)
        {
            return (TargetType) Enum.Parse(typeof(TargetType), stringValue);
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
