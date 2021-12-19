using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.E2ETests
{
    public class FinanceDataMigrationApiControllerTests
    {
        private readonly HttpClient _testClient;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public FinanceDataMigrationApiControllerTests()
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
        public async Task GetReturnsOkResponseWhenDataToMigrate()
        {
            //arrange cause data to be populated for testing

            //act
            var uri = new Uri($"api/v1/data-migration/transaction-entity/extract", UriKind.Relative);
            var response = await _testClient
                .GetAsync(uri).ConfigureAwait(false);

            var result = response.StatusCode;

            //assert
            result.Should().Be(HttpStatusCode.OK);

        }

        [Fact(Skip = "Person API requires authentication but FinanceDataMigrationAPI has not implement auth Brearer")]
        public async Task TransformTransitionEntityReturnsOkResponseWhenDataToMigrate()
        {
            //arrange cause data to be populated for testing

            //act
            var uri = new Uri($"api/v1/data-migration/transaction-entity/transform", UriKind.Relative);
            var response = await _testClient
                .GetAsync(uri).ConfigureAwait(false);

            var result = response.StatusCode;

            //assert
            result.Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public async Task LoadTransactionEntityReturnsOkResponseWhenDataToMigrate()
        {
            //arrange cause data to be populated for testing -seed_data

            //act
            var uri = new Uri($"api/v1/data-migration/transaction-entity/load", UriKind.Relative);
            var response = await _testClient
                .GetAsync(uri).ConfigureAwait(false);

            var result = response.StatusCode;

            //assert
            result.Should().Be(HttpStatusCode.OK);

        }
    }
}
