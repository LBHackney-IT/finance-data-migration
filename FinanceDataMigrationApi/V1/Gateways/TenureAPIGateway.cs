using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Collections.Generic;
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

            var uri = new Uri($"api/v1/search/tenures?searchText={prn}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var tenureResponse = await response.ReadContentAs<APIResponse<TenureResponse>>().ConfigureAwait(true);

            return tenureResponse?.Results.Tenures;
        }
    }
}
