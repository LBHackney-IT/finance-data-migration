using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Controllers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FluentAssertions;
using Hackney.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
//using NUnit.Framework;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Controllers
{
    public class BaseControllerTests
    {
        private BaseController _sut;
        private ControllerContext _controllerContext;
        private HttpContext _stubHttpContext;

        public BaseControllerTests()
        {
            _stubHttpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_stubHttpContext, new RouteData(), new ControllerActionDescriptor()));
            _sut = new BaseController();

            _sut.ControllerContext = _controllerContext;
        }

        [Fact]
        public void GetCorrelationShouldThrowExceptionIfCorrelationHeaderUnavailable()
        {
            // Arrange + Act + Assert
            _sut.Invoking(x => x.GetCorrelationId())
                .Should().Throw<KeyNotFoundException>()
                .WithMessage("Request is missing a correlationId");
        }

        [Fact]
        public void GetCorrelationShouldReturnCorrelationIdWhenExists()
        {
            // Arrange
            _stubHttpContext.Request.Headers.Add(HeaderConstants.CorrelationId, "123");

            // Act
            var result = _sut.GetCorrelationId();

            // Assert
            result.Should().BeEquivalentTo("123");
        }
    }
}
