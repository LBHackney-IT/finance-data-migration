using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FinanceDataMigrationApi.V1.Gateways.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(dataAsString, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        }
        public static async Task<bool> PostAsJsonAsyncType<T>(this HttpClient httpClient, Uri url, T data)
        {
            var formatter = new JsonMediaTypeFormatter {SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };
            var response = await httpClient.PostAsync(url, data, formatter).ConfigureAwait(true);

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
