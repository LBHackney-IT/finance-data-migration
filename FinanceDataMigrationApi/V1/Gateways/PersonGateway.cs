using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Extensions;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using Hackney.Shared.Person;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class PersonGateway: IPersonGateway
    {
        private readonly HttpClient _client;

        public PersonGateway(HttpClient client)
        {
            _client = client;
        }   

        public async Task<Person> GetById(Guid id)
        {
            if(id==Guid.Empty)
                throw  new ArgumentNullException(nameof(id));


            var uri = new Uri($"/api/v1/persons/{id.ToString()}", UriKind.Relative);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            var personResponse = await response.ReadContentAs<Person>().ConfigureAwait(true);

            //return tenureResponse?.Results.Tenures;


            return personResponse;
        }
    }
}
