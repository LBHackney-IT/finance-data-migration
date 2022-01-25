using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.E2ETests
{
    public class HealthCheckControllerTests
    {
        private readonly HttpClient _testClient;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public HealthCheckControllerTests()
        {
            //arrange
            _factory = new CustomWebApplicationFactory<Startup>();
            _factory.SetEnvironmentVariables();
            _testClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task HealthCheckReturnsOk()
        {
            var uri = new Uri($"api/v1/healthcheck/ping", UriKind.Relative);

            var response = await _testClient.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public async Task ThrowErrorReturnException()
        {
            var uri = new Uri($"api/v1/healthcheck/error", UriKind.Relative);

            var response = await _testClient.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
