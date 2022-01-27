using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Controllers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
//using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Controllers
{
    public class FinanceDataMigrationApiControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly FinanceDataMigrationApiController _controller;
        private readonly Mock<IExtractTransactionEntityUseCase> _mockExtractTransactionEntityUseCase;
        private readonly Mock<ITransformTransactionEntityUseCase> _mockTransformTransactionEntityUse;
        private readonly Mock<ILoadTransactionEntityUseCase> _mockLoadTransactionEntityUseCase;
        private readonly Mock<IIndexTransactionEntityUseCase> _mockIndexTransactionEntityUseCaseCase;

        public FinanceDataMigrationApiControllerTests()
        {
            _mockExtractTransactionEntityUseCase = new Mock<IExtractTransactionEntityUseCase>();
            _mockTransformTransactionEntityUse = new Mock<ITransformTransactionEntityUseCase>();
            _mockLoadTransactionEntityUseCase = new Mock<ILoadTransactionEntityUseCase>();
            _mockIndexTransactionEntityUseCaseCase = new Mock<IIndexTransactionEntityUseCase>();

            _controller = new FinanceDataMigrationApiController(
                _mockExtractTransactionEntityUseCase.Object,
                _mockTransformTransactionEntityUse.Object,
                _mockLoadTransactionEntityUseCase.Object,
                _mockIndexTransactionEntityUseCaseCase.Object);
        }

        [Fact]
        public async Task ExtractTransactionEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockExtractTransactionEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.ExtractTransactionEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task TransformTransactionEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockTransformTransactionEntityUse.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.TransformTransactionEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task LoadTransactionEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockLoadTransactionEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.LoadTransactionEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

    }
}
