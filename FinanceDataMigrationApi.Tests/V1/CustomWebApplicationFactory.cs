using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinanceDataMigrationApi.Tests.V1
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder() =>
            base.CreateWebHostBuilder().UseEnvironment("Development");

        private JsonTextReader _jsonReader;
        protected internal void SetEnvironmentVariables()
        {
            using var launchSettingsFile = File.OpenText("./Properties/launchSettings.json");
            // Load environment variables from json configuration file
            _jsonReader = new JsonTextReader(reader: launchSettingsFile);
            JObject jsonObject = JObject.Load(reader: _jsonReader);

            // Get variables from config file into List
            List<JProperty> envVariables = jsonObject
                .GetValue("profiles")
                .SelectMany(profiles => profiles.Children())
                .SelectMany(profile => profile.Children<JProperty>())
                .Where(property => property.Name == "environmentVariables")
                .SelectMany(property => property.Value.Children<JProperty>())
                .ToList();

            // Manually set each variable read from config file
            envVariables.ForEach(variable =>
            {
                Environment.SetEnvironmentVariable(
                    variable: variable.Name,
                    value: variable.Value.ToString());
            });

            launchSettingsFile.Close();
        }
    }
}
