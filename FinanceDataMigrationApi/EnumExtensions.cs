using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.ComponentModel;
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
            // Or return default(T);
        }

        //public static T GetValueFromDescription<T>(string description) where T : Enum
        //{
        //    foreach (var field in typeof(T).GetFields())
        //    {
        //        if (Attribute.GetCustomAttribute(field,
        //        typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        //        {
        //            if (attribute.Description == description)
        //                return (T) field.GetValue(null);
        //        }
        //        else
        //        {
        //            if (field.Name == description)
        //                return (T) field.GetValue(null);
        //        }
        //    }

        //    throw new ArgumentException("Not found.", nameof(description));
        //    // Or return default(T);
        //}
    }
}
