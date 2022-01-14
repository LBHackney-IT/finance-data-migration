using FinanceDataMigrationApi.V1.Handlers;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using System;

namespace FinanceDataMigrationApi.V1.Infrastructure.Extentions
{
    public static class EnumsExtentions
    {
        public static T ToEnumValue<T>(this string stringValue)
        {
            try
            {
                if (stringValue == null)
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
        public static RentGroupType ToRentGroup (this string stringValue)
        {
            try
            {
                if (stringValue == null)
                {
                    return default(RentGroupType);
                }
                var trimmedStringValue = stringValue.Trim();
                switch (trimmedStringValue)
                {
                    case "Gar & Park HRA":
                        return RentGroupType.Garages;
                    case "Housing Gen Fund":
                        return RentGroupType.GenFundRents;
                    case "Housing Revenue":
                        return RentGroupType.HraRents;
                    case "LH Major Works":
                        return RentGroupType.MajorWorks;
                    case "LH Serv Charges":
                        return RentGroupType.LeaseHolders;
                    case "Temp Acc Gen Fun":
                        return RentGroupType.TempAcc;
                    case "Travel Gen Fund":
                        return RentGroupType.Travelers;
                    case "Temp Accom HRA":
                        return RentGroupType.TempAccHRA;
                    default:
                        throw new ArgumentException("Exception was thrown in ToRentGroup method " +
                            "while attempting converting rent group string representation to enum " +
                            "representation in 'MSSQL to ES' transferring."); ;
                }
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{typeof(RentGroupType)} : {e.Message}");
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }
    }
}
