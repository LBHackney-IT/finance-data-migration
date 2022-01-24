using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Controllers;
using FinanceDataMigrationApi.V1.UseCase;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Controllers
{
    public class HealthCheckControllerTests
    {
        private HealthCheckController _classUnderTest;

        public HealthCheckControllerTests()
        {
            _classUnderTest = new HealthCheckController();
        }

        [Fact]
        public void ReturnsResponseWithStatus()
        {
            var expected = new Dictionary<string, object> { { "success", true } };
            var response = _classUnderTest.HealthCheck() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(expected);
        }
    }
}
