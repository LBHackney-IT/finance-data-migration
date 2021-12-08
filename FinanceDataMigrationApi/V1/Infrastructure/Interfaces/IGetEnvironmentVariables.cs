using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Infrastructure.Interfaces
{
    public interface IGetEnvironmentVariables
    {
        public string GetPersonApiUrl();
        public string GetPersonApiToken();
        public string GetHousingSearchApi(ESearchBy searchBy);
        public string GetHousingSearchApiToken();
    }
}
