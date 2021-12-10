using System;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public class GetEnvironmentVariables : IGetEnvironmentVariables
    {
        public string GetPersonApiUrl()
        {
            string result = Environment.GetEnvironmentVariable("PERSON_API_URL") ?? string.Empty; ;
            if (string.IsNullOrEmpty(result))
                throw new Exception("Person api url shouldn't be null or empty.");
            return result;
        }

        public string GetPersonApiToken()
        {
            string result = Environment.GetEnvironmentVariable("PERSON_API_TOKEN") ?? string.Empty; ;
            if (string.IsNullOrEmpty(result))
                throw new Exception("Person api token shouldn't be null or empty.");
            return result;
        }

        public string GetHousingSearchApi(ESearchBy searchBy)
        {
            string result = Environment.GetEnvironmentVariable("SEARCH_API_URL") ?? string.Empty; ;
            if (string.IsNullOrEmpty(result))
                throw new Exception("Search api url shouldn't be null or empty");
            switch (searchBy)
            {
                case ESearchBy.ByAccount:
                    return result + $"/api/v1/search/accounts";
                case ESearchBy.ByAsset:
                    return result + $"/api/v1/search/assets";
                case ESearchBy.ByCharge:
                    return result + $"/api/v1/search/charges";
                case ESearchBy.ByPerson:
                    return result + $"/api/v1/search/persons";
                case ESearchBy.ByTenure:
                    return result + $"/api/v1/search/tenures";
                case ESearchBy.ByTransaction:
                    return result + $"/api/v1/search/transactions";
                default:
                    throw new ArgumentNullException($"{nameof(searchBy).ToString()} is not valid");
            }
        }

        public string GetHousingSearchApiToken()
        {
            string result = Environment.GetEnvironmentVariable("SEARCH_API_TOKEN") ?? string.Empty; ;
            if (string.IsNullOrEmpty(result))
                throw new Exception("Search api authorization api key shouldn't be null or empty.");
            return result;
        }
    }
}
