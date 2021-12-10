using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using Hackney.Shared.Tenure.Domain;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway: ITenureGateway
    {
        private readonly ICustomeHttpClient _client;
        private readonly IGetEnvironmentVariables _getEnvironmentVariables;

        public TenureGateway(ICustomeHttpClient client,IGetEnvironmentVariables getEnvironmentVariables)
        {
            _client = client;
            _getEnvironmentVariables = getEnvironmentVariables;
        }

        public async Task<List<TenureInformation>> GetByPrnAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            var searchApiUrl = _getEnvironmentVariables.GetHousingSearchApi(ESearchBy.ByTenure).ToString();
            var searchAuthKey = _getEnvironmentVariables.GetHousingSearchApiToken();

            _client.AddHeader(new HttpHeader<string, string> { Name = "Authorization", Value = searchAuthKey });

            var response = await _client.GetAsync(new Uri($"{searchApiUrl}?searchText={prn}")).ConfigureAwait(false);
            if (response == null)
            {
                throw new Exception("The search api is not reachable!");
            }
            else if (response.Content == null)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var tenureResponse = JsonConvert.DeserializeObject<APIResponse<TenureResponse>>(responseContent);
            return tenureResponse?.Results.Tenures;
        }
    }
}
