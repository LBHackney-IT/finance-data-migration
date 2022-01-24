using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureAPIGateway : ITenureAPIGateway
    {
        private readonly HttpClient _client;

        public TenureAPIGateway(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<TenureInformation>> GetByPrnAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            var uri = new Uri($"development/api/v1/search/tenures?searchText={prn}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var tenureResponse = await response.ReadContentAs<APIResponse<TenureResponse>>().ConfigureAwait(true);

            return tenureResponse?.Results.Tenures;
        }

        public async Task<List<TenureInformation>> GetTenuresByPrnAsync(List<string> prnList)
        {
            var uri = new Uri($"v1/search/tenures/byPrnList?{ConstructQueryParameters(prnList)}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);

            var tenureResponse = await response.ReadContentAs<APIResponse<TenureResponse>>().ConfigureAwait(true);

            return tenureResponse?.Results.Tenures;
        }

        private static string ConstructQueryParameters(List<string> prnList)
        {
            if (prnList == null || !prnList.Any())
            {
                return string.Empty;
            }
            var queryParameters = "prnList=" + prnList[0];

            if (prnList.Count > 1)
            {
                queryParameters += "&prnList=" + string.Join("&prnList=", prnList.Skip(1).Take(prnList.Count - 1));
            }

            return queryParameters;
        }
    }
}
