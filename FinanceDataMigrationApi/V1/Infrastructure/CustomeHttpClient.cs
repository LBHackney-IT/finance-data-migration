using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces; 

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public class CustomeHttpClient : HttpClient, ICustomeHttpClient
    {
        public void AddAuthorization(AuthenticationHeaderValue headerValue)
        {
            if (headerValue != null) base.DefaultRequestHeaders.Authorization = headerValue;
        }

        public void AddHeader(HttpHeader<string, string> header)
        {
            DefaultRequestHeaders.Add(header.Name, header.Value);
        }

 
        public new async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return await base.GetAsync(uri).ConfigureAwait(false);
        }
    }
}
