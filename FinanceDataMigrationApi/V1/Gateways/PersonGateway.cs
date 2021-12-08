using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using Hackney.Shared.Person;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class PersonGateway: IPersonGateway
    {
        private readonly ICustomeHttpClient _client;
        private readonly IGetEnvironmentVariables _getEnvironmentVariables;

        public PersonGateway(ICustomeHttpClient client,IGetEnvironmentVariables getEnvironmentVariables)
        {
            _client = client;
            _getEnvironmentVariables = getEnvironmentVariables;
        }

        public async Task<Person> GetById(Guid id)
        {
            if(id==Guid.Empty)
                throw  new ArgumentNullException(nameof(id));

            var personApiUrl = _getEnvironmentVariables.GetPersonApiUrl();
            var personAuthKey = _getEnvironmentVariables.GetPersonApiToken();

            /*_client.AddHeader(new HttpHeader<string, string> { Name = "Authorization", Value = personAuthKey });*/
            _client.AddAuthorization(new AuthenticationHeaderValue("Bearer", personAuthKey));

            var response = await _client.GetAsync(new Uri($"{personApiUrl}/{id.ToString()}")).ConfigureAwait(false);
            if (response == null)
            {
                throw new Exception("The person api is not reachable!");
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
