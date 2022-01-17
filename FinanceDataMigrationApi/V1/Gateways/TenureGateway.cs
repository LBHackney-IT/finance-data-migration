using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Tenure;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway: ITenureGateway
    {
        private readonly HttpClient _client;

        public TenureGateway(HttpClient client)
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

        public Task<TenureInformation> GetEntityById(TenureQueryRequest query)
        {
            throw new NotImplementedException();
        }
    }
}
