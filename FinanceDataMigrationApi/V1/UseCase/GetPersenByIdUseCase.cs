using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.Person;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetPersenByIdUseCase: IGetPersenByIdUseCase
    {
        private readonly ICustomeHttpClient _client;
        private readonly IGetEnvironmentVariables _getEnvironmentVariables;

        public GetPersenByIdUseCase(ICustomeHttpClient client,IGetEnvironmentVariables getEnvironmentVariables)
        {
            _client = client;
            _getEnvironmentVariables = getEnvironmentVariables;
        }
        public async Task<Person> ExecuteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException($"the {nameof(id).ToString()} shouldn't be empty or null");

            var personApiUrl = _getEnvironmentVariables.GetPersonApiUrl().ToString();
            var personApiToken = _getEnvironmentVariables.GetPersonApiToken();

            _client.AddAuthorization(new AuthenticationHeaderValue("Bearer", personApiToken));

            var response = await _client.GetAsync(new Uri($"{personApiUrl}/api/v1/persons/{id.ToString()}")).ConfigureAwait(false);
            if (response == null)
            {
                throw new Exception($"The person api is not reachable!{personApiUrl}");
            }
            else if (response.Content == null)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var personResponse = JsonConvert.DeserializeObject<Person>(responseContent);
            return personResponse;
        }
    }
}
