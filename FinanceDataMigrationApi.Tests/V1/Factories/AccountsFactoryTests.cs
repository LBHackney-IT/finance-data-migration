using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain.Accounts;
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
        private readonly DMAccountEntity _dmAccountEntityWithNullTenureAndConsolidatetCharges;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelWhenTenureIsNullAndConsolidatedChargesIsNull;

        private readonly DMAccountEntity _dmAccountEntityWithNullNotNullTenure;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelWhenTenureIsNotNull;
        private readonly TenureDbEntity _tenure;

        private readonly DMAccountEntity _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelWhenTenureIsNotNullAndNotNullTenureType;

        private readonly DMAccountEntity _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelWhenTenureIsNotNullAndNotNullPrimaryTenants;
        private readonly TenureDbEntity _tenureWhenTenureIsNotNullAndNotNullPrimaryTenants;

        private readonly DMAccountEntity _dmAccountEntityWithNullNotNullConsolidatedCharges;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelWhenConsolidatedChargesIsNotNull;

        public AccountsFactoryTests()
        {
            #region ToQueryRequestTenureIsNullAndConsolidatedChargesIsNullShouldReturnsAccountModel
            _dmAccountEntityWithNullTenureAndConsolidatetCharges = new DMAccountEntity()
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

            _expectedAccountModelWhenTenureIsNullAndConsolidatedChargesIsNull = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityWithNullTenureAndConsolidatetCharges.AccountBalance.HasValue ? _dmAccountEntityWithNullTenureAndConsolidatetCharges.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityWithNullTenureAndConsolidatetCharges.ParentAccountId.ToString()}}
            };
            #endregion ToQueryRequestTenureIsNullAndConsolidatedChargesIsNullShouldReturnsAccountModel

            #region ToQueryRequestTenureIsNotNullShouldReturnsAccountModel
            _dmAccountEntityWithNullNotNullTenure = new DMAccountEntity()
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
                Tenure = @"{ ""tenureId"":""318229C6-C70C-4E0F-9226-35C01BD8471F"", ""fullAddress"":""sdfsdfsdfsdfsdf""}",
                AccountStatus = string.Empty,
                ParentAccountId = Guid.NewGuid(),
                IsTransformed = false,
                IsLoaded = false,
                DynamoDbId = Guid.NewGuid(),
                ConsolidatedCharges = string.Empty
            };

            _tenure = new TenureDbEntity()
            {
                TenureId = "318229C6-C70C-4E0F-9226-35C01BD8471F",
                FullAddress = "sdfsdfsdfsdfsdf",
                TenureType = new TenureTypeDbEntity()
                {
                    Code = "1941079102",
                    Description = "kjdngk jfkjfnkjdfv"
                },
            };

            _expectedAccountModelWhenTenureIsNotNull = new Dictionary<string, AttributeValue>
            {
                 {"id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityWithNullNotNullTenure.AccountBalance.HasValue ? _dmAccountEntityWithNullTenureAndConsolidatetCharges.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenure.ParentAccountId.ToString()}},
                {"tenure", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "tenureId", new AttributeValue { S = _tenure.TenureId }},
                            { "fullAddress", new AttributeValue { S = _tenure.FullAddress }}
                        }
                    }
                }
            };
            #endregion

            #region ToQueryRequestTenureIsNotNullTenureTypeIsNotNullShouldReturnsAccountModel
            _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType = new DMAccountEntity()
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
                Tenure = @"{ ""tenureId"":""318229C6-C70C-4E0F-9226-35C01BD8471F"", ""fullAddress"":""sdfsdfsdfsdfsdf"",
                ""tenureType"":{
                ""code"":""1941079102"", ""description"":""kjdngk jfkjfnkjdfv""}}",
                AccountStatus = string.Empty,
                ParentAccountId = Guid.NewGuid(),
                IsTransformed = false,
                IsLoaded = false,
                DynamoDbId = Guid.NewGuid(),
                ConsolidatedCharges = string.Empty
            };

            _tenure = new TenureDbEntity()
            {
                TenureId = "318229C6-C70C-4E0F-9226-35C01BD8471F",
                FullAddress = "sdfsdfsdfsdfsdf",
                TenureType = new TenureTypeDbEntity()
                {
                    Code = "1941079102",
                    Description = "kjdngk jfkjfnkjdfv"
                },
            };

            _expectedAccountModelWhenTenureIsNotNullAndNotNullTenureType = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.AccountBalance.HasValue ? _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.ParentAccountId.ToString()}},
                {"tenure", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "tenureId", new AttributeValue { S = _tenure.TenureId }},
                            { "fullAddress", new AttributeValue { S = _tenure.FullAddress }},
                            { "tenureType", new AttributeValue
                                {
                                    M = new Dictionary<string, AttributeValue>
                                    {
                                        {"code", new AttributeValue {S = _tenure.TenureType.Code}},
                                        {"description", new AttributeValue {S = _tenure.TenureType.Description}},
                                    }
                                }
                            }
                        }
                    }
                }
            };
            #endregion

            #region ToQueryRequestTenureIsNotNullPrimaryTenantsIsNotNullShouldReturnsAccountModel
            _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants = new DMAccountEntity()
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
                AccountStatus = string.Empty,
                ParentAccountId = Guid.NewGuid(),
                IsTransformed = false,
                IsLoaded = false,
                DynamoDbId = Guid.NewGuid(),
                ConsolidatedCharges = string.Empty,
                Tenure = @"{ ""tenureId"":""318229C6-C70C-4E0F-9226-35C01BD8471F"", ""fullAddress"":""sdfsdfsdfsdfsdf"",
                ""primaryTenants"":[{
                ""id"":""76F78616-B8ED-4703-9E93-D808E21B570F"",""fullName"":""3A398F08-4712-4E78-A641-96E6F9301094""},{
                ""id"":""9AFCB18A-F500-4F3A-A784-D4B3A2B59678"",""fullName"":""BC18422C-3263-4BAB-8CA3-25A43C2A84CE""}]}",
            };

            _tenureWhenTenureIsNotNullAndNotNullPrimaryTenants = new TenureDbEntity()
            {
                TenureId = "318229C6-C70C-4E0F-9226-35C01BD8471F",
                FullAddress = "sdfsdfsdfsdfsdf",
                TenureType = new TenureTypeDbEntity()
                {
                    Code = "1941079102",
                    Description = "kjdngk jfkjfnkjdfv"
                },
                PrimaryTenants = new List<PrimaryTenantsDbEntity>()
                {
                    new PrimaryTenantsDbEntity()
                    {
                        Id = new Guid("76F78616-B8ED-4703-9E93-D808E21B570F"),
                        FullName = "3A398F08-4712-4E78-A641-96E6F9301094",
                    },
                     new PrimaryTenantsDbEntity()
                    {
                        Id = new Guid("9AFCB18A-F500-4F3A-A784-D4B3A2B59678"),
                        FullName = "BC18422C-3263-4BAB-8CA3-25A43C2A84CE",
                    },
                }
            };

            _expectedAccountModelWhenTenureIsNotNullAndNotNullPrimaryTenants = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.AccountBalance.HasValue ? _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.ParentAccountId.ToString()}},
                {"primaryTenants",
                    new AttributeValue
                    {
                        L = new List<AttributeValue>(_tenureWhenTenureIsNotNullAndNotNullPrimaryTenants.PrimaryTenants.Count)
                        {
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"fullName", new AttributeValue {S = "3A398F08-4712-4E78-A641-96E6F9301094" }},
                                    {"id", new AttributeValue {S = "76F78616-B8ED-4703-9E93-D808E21B570F" }}
                                }
                            },
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"fullName", new AttributeValue {S = "BC18422C-3263-4BAB-8CA3-25A43C2A84CE" }},
                                    {"id", new AttributeValue {S = "9AFCB18A-F500-4F3A-A784-D4B3A2B59678" }}
                                }
                            },
                        }
                    }
                },
            };
            #endregion

            #region ToQueryRequestConsolidatedChargesIsNotNullShouldReturnsAccountModel
            _dmAccountEntityWithNullNotNullConsolidatedCharges = new DMAccountEntity()
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
                AccountStatus = string.Empty,
                ParentAccountId = Guid.NewGuid(),
                IsTransformed = false,
                IsLoaded = false,
                DynamoDbId = Guid.NewGuid(),
                ConsolidatedCharges = @"[{""type"":""A"", ""frequency"":""WK"", ""amount"":""10.70""}, { ""type"":""A"", ""frequency"":""WK"", ""amount"":""11.00"" }]",
                Tenure = string.Empty
            };

            _expectedAccountModelWhenConsolidatedChargesIsNotNull = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityWithNullNotNullTenureAndNotNullPrimaryTenants.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityWithNullNotNullConsolidatedCharges.AccountBalance.HasValue ? _dmAccountEntityWithNullNotNullConsolidatedCharges.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityWithNullNotNullConsolidatedCharges.ParentAccountId.ToString()}},
                {"consolidatedCharges",
                    new AttributeValue
                    {
                        L = new List<AttributeValue>(2)
                        {
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"amount", new AttributeValue {N = "10.70" }},
                                    {"frequency", new AttributeValue {S = "WK" }},
                                    {"type", new AttributeValue {S = "A" }},
                                }
                            },
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"amount", new AttributeValue {N = "11.00" }},
                                    {"frequency", new AttributeValue {S = "WK" }},
                                    {"type", new AttributeValue {S = "A" }},
                                }
                            },
                        }
                    }
                }
            };
            #endregion
        }

        [Fact]
        public void ToQueryRequestTenureIsNullAndConsolidatedChargesIsNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityWithNullTenureAndConsolidatetCharges.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelWhenTenureIsNullAndConsolidatedChargesIsNull);
        }

        [Fact]
        public void ToQueryRequestTenureIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityWithNullNotNullTenure.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelWhenTenureIsNotNull);
        }

        [Fact]
        public void ToQueryRequestTenureIsNotNullTenureTypeIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelWhenTenureIsNotNullAndNotNullTenureType);
        }

        [Fact]
        public void ToQueryRequestTenureIsNotNullPrimaryTenantsIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityWithNullNotNullTenureAndNotNullTenureType.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelWhenTenureIsNotNullAndNotNullTenureType);
        }

        [Fact]
        public void ToQueryRequestConsolidatedChargesIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityWithNullNotNullConsolidatedCharges.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelWhenConsolidatedChargesIsNotNull);
        }
    }
}
