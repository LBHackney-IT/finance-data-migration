using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Factories
{
    public class AccountsFactoryTests
    {
        private readonly DMAccountEntity _dmAccountEntity;
        public AccountsFactoryTests()
        {
            _dmAccountEntity = new DMAccountEntity()
            {
                Id = long.MaxValue,
                TargetType = string.Empty,
                TargetId = Guid.NewGuid(),
                AccountType = string.Empty,
                AgreementType = string.Empty,
                RentGroupType = string.Empty,
                PaymentReference = string.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                EndReasonCode = string.Empty,
                Tenure = string.Empty,
                AccountStatus = string.Empty,
                ParentAccountId = Guid.NewGuid(),
                IsTransformed = false,
                IsLoaded = false,
                DynamoDbId = Guid.NewGuid(),
                ConsolidatedCharges = string.Empty
            };
        }

        [Fact]
        public void ToQueryRequestShouldReturnsAccountModel()
        {
            var result = _dmAccountEntity.ToQueryRequest();

            result.Should().BeOfType<Dictionary<string, AttributeValue>>();
        }
    }
}
