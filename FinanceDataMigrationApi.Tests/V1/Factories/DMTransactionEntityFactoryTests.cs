using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Infrastructure;
using FluentAssertions;
using System;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Factories
{
    public class DMTransactionEntityFactoryTests
    {
        private readonly DMTransactionEntityDomain _dMTransactionEntityDomain;
        private readonly DMTransactionEntityDomain _dMTransactionEntityDomainWithNull;
        private readonly DMTransactionEntity _dMTransactionEntity;
        private readonly DMTransactionEntity _dMTransactionEntityWithNull;

        public DMTransactionEntityFactoryTests()
        {
            _dMTransactionEntityDomain = new DMTransactionEntityDomain
            {
                Id = long.MaxValue,
                BalanceAmount = decimal.MaxValue,
                ChargedAmount = decimal.MaxValue,
                CreatedAt = DateTimeOffset.MaxValue,
                FinancialMonth = short.MaxValue,
                FinancialYear = short.MaxValue,
                Fund = string.Empty,
                HousingBenefitAmount = decimal.MaxValue,
                IdDynamodb = Guid.NewGuid(),
                IsLoaded = false,
                IsIndexed = false,
                IsSuspense = false,
                IsTransformed = false,
                PaidAmount = decimal.MaxValue,
                PaymentReference = string.Empty,
                PeriodNo = int.MaxValue,
                Person = string.Empty,
                SuspenseResolutionInfo = string.Empty,
                TargetId = Guid.NewGuid(),
                TargetType = string.Empty,
                TransactionAmount = decimal.MaxValue,
                TransactionDate = DateTime.Now,
                TransactionSource = string.Empty,
                TransactionType = string.Empty
            };
            _dMTransactionEntityDomainWithNull = null;
            _dMTransactionEntity = new DMTransactionEntity
            {
                Id = long.MaxValue,
                BalanceAmount = decimal.MaxValue,
                BankAccountNumber = string.Empty,
                ChargedAmount = decimal.MaxValue,
                CreatedAt = DateTimeOffset.Now,
                FinancialMonth = int.MaxValue,
                FinancialYear = int.MaxValue,
                Fund = string.Empty,
                HousingBenefitAmount = decimal.MaxValue,
                IdDynamodb = Guid.NewGuid(),
                IsLoaded = true,
                IsIndexed = false,
                IsSuspense = true,
                IsTransformed = false,
                PaidAmount = decimal.MaxValue,
                PaymentReference = string.Empty,
                PeriodNo = int.MaxValue,
                Person = string.Empty,
                SuspenseResolutionInfo = string.Empty,
                TargetId = Guid.Empty,
                TargetType = string.Empty,
                TransactionAmount = decimal.MaxValue,
                TransactionDate = DateTime.Now,
                TransactionSource = string.Empty,
                TransactionType = string.Empty
            };
            _dMTransactionEntityWithNull = null;
        }

        [Fact]
        public void ToDatabaseShouldReturnsDmTransactionEntity()
        {
            var result = _dMTransactionEntityDomain.ToDatabase();

            result.Should().BeOfType<DMTransactionEntity>();
        }

        [Fact]
        public void ToDatabaseShouldReturnsNull()
        {
            var result = _dMTransactionEntityDomainWithNull.ToDatabase();

            result.Should().BeNull();
        }

        [Fact]
        public void ToDomainShouldReturnsDmTransactionEntityDomain()
        {
            var result = _dMTransactionEntity.ToDomain();

            result.Should().BeOfType<DMTransactionEntityDomain>();
        }

        [Fact]
        public void ToDomainShouldReturnsNull()
        {
            var result = _dMTransactionEntityWithNull.ToDomain();

            result.Should().BeNull();
        }
    }
}
