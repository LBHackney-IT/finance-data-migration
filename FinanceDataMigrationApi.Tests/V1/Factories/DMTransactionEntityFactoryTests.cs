using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Factories
{
    public class DMTransactionEntityFactoryTests
    {
        private readonly DMTransactionEntityDomain _dMTransactionEntityDomain;
        private readonly DMTransactionEntityDomain _dMTransactionEntityDomainWithNull;
        private readonly DMTransactionEntity _dMTransactionEntity;
        private readonly DMTransactionEntity _dMTransactionEntityWithNull;
        private readonly IList<DMTransactionEntity> _listOfDbEntities;
        private readonly IList<DMTransactionEntityDomain> _listOfDbDomainEntities;

        public DMTransactionEntityFactoryTests()
        {
            _dMTransactionEntityDomain = new DMTransactionEntityDomain
            {
                Id = long.MaxValue,
                BalanceAmount = decimal.MaxValue,
                BankAccountNumber = string.Empty,
                ChargedAmount = decimal.MaxValue,
                CreatedAt = DateTimeOffset.MaxValue,
                FinancialMonth = 12,
                FinancialYear = 32767,
                Fund = string.Empty,
                HousingBenefitAmount = decimal.MaxValue,
                IdDynamodb = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                IsLoaded = false,
                IsIndexed = false,
                IsSuspense = false,
                IsTransformed = false,
                PaidAmount = decimal.MaxValue,
                PaymentReference = string.Empty,
                PeriodNo = int.MaxValue,
                Person = string.Empty,
                SuspenseResolutionInfo = string.Empty,
                TargetId = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                TargetType = string.Empty,
                TransactionAmount = decimal.MaxValue,
                TransactionDate = DateTime.Today,
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
                CreatedAt = DateTimeOffset.MaxValue,
                FinancialMonth = 12,
                FinancialYear = 32767,
                Fund = string.Empty,
                HousingBenefitAmount = decimal.MaxValue,
                IdDynamodb = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                IsLoaded = false,
                IsIndexed = false,
                IsSuspense = false,
                IsTransformed = false,
                PaidAmount = decimal.MaxValue,
                PaymentReference = string.Empty,
                PeriodNo = int.MaxValue,
                Person = string.Empty,
                SuspenseResolutionInfo = string.Empty,
                TargetId = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                TargetType = string.Empty,
                TransactionAmount = decimal.MaxValue,
                TransactionDate = DateTime.Today,
                TransactionSource = string.Empty,
                TransactionType = string.Empty
            };
            _dMTransactionEntityWithNull = null;

            _listOfDbEntities = new List<DMTransactionEntity>
            {
                _dMTransactionEntity
            };
            _listOfDbDomainEntities = new List<DMTransactionEntityDomain>
            {
                _dMTransactionEntityDomain
            };
        }

        [Fact]
        public void ToDatabaseShouldReturnsDmTransactionEntity()
        {
            var result = _dMTransactionEntityDomain.ToDatabase();

            result.Should().BeEquivalentTo(_dMTransactionEntity);
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

            result.Should().BeEquivalentTo(_dMTransactionEntityDomain);
        }

        [Fact]
        public void ToDomainShouldReturnsNull()
        {
            var result = _dMTransactionEntityWithNull.ToDomain();

            result.Should().BeNull();
        }

        [Fact]
        public void ToDomainShouldReturnsListOfDMDomainTransactionEntityModels()
        {
            var result = _listOfDbEntities.ToDomain();

            result.Should().BeEquivalentTo(_listOfDbDomainEntities);
        }

        [Fact]
        public void ToDomainShouldReturnsListOfDMTransactionEntityModels()
        {
            var result = _listOfDbDomainEntities.ToDatabase();

            result.Should().BeEquivalentTo(_listOfDbEntities);
        }
    }
}
