using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Infrastructure.Interfaces
{
    public interface ICustomeHttpClient
    {
        public void AddAuthorization(AuthenticationHeaderValue headerValue);
        public void AddHeader(HttpHeader<string, string> header);
        public Task<HttpResponseMessage> GetAsync(Uri uri);
    }

    public class HttpHeader<T, T1>
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
