using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using TargetType = FinanceDataMigrationApi.V1.Domain.TargetType;

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

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T) field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T) field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }
    }
}
