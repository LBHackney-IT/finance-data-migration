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
        private readonly DmAccountDbEntity _dmAccountEntitySubmodelsAreNull;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelSubmodelsAreNull;

        private readonly DmAccountDbEntity _dmAccountEntityTenureIsNotNull;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelTenureIsNotNull;

        private readonly DmAccountDbEntity _dmAccountEntityPopulatedTenure;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelPopulatedTenure;

        private readonly DmAccountDbEntity _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelTenureIsNotNullAndNotNullPrimaryTenants;

        private readonly DmAccountDbEntity _dmAccountEntityConsolidatedChargesIsNotNull;
        private readonly Dictionary<string, AttributeValue> _expectedAccountModelConsolidatedChargesIsNotNull;

        public AccountsFactoryTests()
        {
            #region SubmodelsAreNullShouldReturnsAccountModel
            _dmAccountEntitySubmodelsAreNull = new DmAccountDbEntity()
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

            _expectedAccountModelSubmodelsAreNull = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntitySubmodelsAreNull.AccountBalance.HasValue ? _dmAccountEntitySubmodelsAreNull.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntitySubmodelsAreNull.ParentAccountId.ToString()}}
            };
            #endregion ToQueryRequestTenureIsNullAndConsolidatedChargesIsNullShouldReturnsAccountModel

            #region TenureIsNotNullShouldReturnsAccountModel
            _dmAccountEntityTenureIsNotNull = new DmAccountDbEntity()
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

            _expectedAccountModelTenureIsNotNull = new Dictionary<string, AttributeValue>
            {
                 {"id", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityTenureIsNotNull.AccountBalance.HasValue ? _dmAccountEntitySubmodelsAreNull.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityTenureIsNotNull.ParentAccountId.ToString()}},
                {"tenure", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "tenureId", new AttributeValue { S = "318229C6-C70C-4E0F-9226-35C01BD8471F" }},
                            { "fullAddress", new AttributeValue { S = "sdfsdfsdfsdfsdf" }}
                        }
                    }
                }
            };
            #endregion

            #region PopulatedTenureShouldReturnsAccountModel
            _dmAccountEntityPopulatedTenure = new DmAccountDbEntity()
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

            _expectedAccountModelPopulatedTenure = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityPopulatedTenure.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityPopulatedTenure.AccountBalance.HasValue ? _dmAccountEntityPopulatedTenure.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityPopulatedTenure.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityPopulatedTenure.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityPopulatedTenure.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityPopulatedTenure.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityPopulatedTenure.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityPopulatedTenure.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityPopulatedTenure.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityPopulatedTenure.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityPopulatedTenure.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityPopulatedTenure.ParentAccountId.ToString()}},
                {"tenure", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "tenureId", new AttributeValue { S = "318229C6-C70C-4E0F-9226-35C01BD8471F" }},
                            { "fullAddress", new AttributeValue { S = "sdfsdfsdfsdfsdf" }},
                            { "tenureType", new AttributeValue
                                {
                                    M = new Dictionary<string, AttributeValue>
                                    {
                                        {"code", new AttributeValue {S = "1941079102"}},
                                        {"description", new AttributeValue {S = "kjdngk jfkjfnkjdfv"}},
                                    }
                                }
                            }
                        }
                    }
                }
            };
            #endregion

            #region TenureIsNotNullPrimaryTenantsIsNotNullShouldReturnsAccountModel
            _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants = new DmAccountDbEntity()
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
                ""id"":""76f78616-b8ed-4703-9e93-d808e21b570f"",""fullName"":""3A398F08-4712-4E78-A641-96E6F9301094""},{
                ""id"":""9afcb18a-f500-4f3a-a784-d4B3a2b59678"",""fullName"":""BC18422C-3263-4BAB-8CA3-25A43C2A84CE""}]}",
            };

            _expectedAccountModelTenureIsNotNullAndNotNullPrimaryTenants = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.AccountBalance.HasValue ? _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.ParentAccountId.ToString()}},
                {"tenure", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "tenureId", new AttributeValue { S = "318229C6-C70C-4E0F-9226-35C01BD8471F" }},
                            { "fullAddress", new AttributeValue { S = "sdfsdfsdfsdfsdf" }},
                            {"primaryTenants",
                                new AttributeValue
                                {
                                    L = new List<AttributeValue>(2)
                                    {
                                        new AttributeValue
                                        {
                                            M = new Dictionary<string, AttributeValue>
                                            {
                                                {"fullName", new AttributeValue {S = "3A398F08-4712-4E78-A641-96E6F9301094" }},
                                                {"id", new AttributeValue {S = "76f78616-b8ed-4703-9e93-d808e21b570f" }}
                                            }
                                        },
                                        new AttributeValue
                                        {
                                            M = new Dictionary<string, AttributeValue>
                                            {
                                                {"fullName", new AttributeValue {S = "BC18422C-3263-4BAB-8CA3-25A43C2A84CE" }},
                                                {"id", new AttributeValue {S = "9afcb18a-f500-4f3a-a784-d4b3a2b59678" }}
                                            }
                                        },
                                    }
                                }
                            },
                        }
                    }
                }
            };
            #endregion

            #region ConsolidatedChargesIsNotNullShouldReturnsAccountModel
            _dmAccountEntityConsolidatedChargesIsNotNull = new DmAccountDbEntity()
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

            _expectedAccountModelConsolidatedChargesIsNotNull = new Dictionary<string, AttributeValue>
            {
                {"id", new AttributeValue {S = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.Id.ToString()}},
                {"account_balance", new AttributeValue {N = _dmAccountEntityConsolidatedChargesIsNotNull.AccountBalance.HasValue ? _dmAccountEntityConsolidatedChargesIsNotNull.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.AgreementType.ToString()}},
                {"start_date", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = _dmAccountEntityConsolidatedChargesIsNotNull.ParentAccountId.ToString()}},
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
        public void SubmodelsAreNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntitySubmodelsAreNull.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelSubmodelsAreNull);
        }

        [Fact]
        public void TenureIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityTenureIsNotNull.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelTenureIsNotNull);
        }

        [Fact]
        public void PopulatedTenureShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityPopulatedTenure.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelPopulatedTenure);
        }

        [Fact]
        public void TenureIsNotNullPrimaryTenantsIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityTenureIsNotNullAndNotNullPrimaryTenants.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelTenureIsNotNullAndNotNullPrimaryTenants);
        }

        [Fact]
        public void ConsolidatedChargesIsNotNullShouldReturnsAccountModel()
        {
            var result = _dmAccountEntityConsolidatedChargesIsNotNull.ToQueryRequest();

            result.Should().BeEquivalentTo(_expectedAccountModelConsolidatedChargesIsNotNull);
        }
    }
}
