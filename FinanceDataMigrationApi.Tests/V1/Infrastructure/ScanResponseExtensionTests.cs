using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Infrastructure;
using FluentAssertions;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Infrastructure
{
    public class ScanResponseExtensionTests
    {
        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTenuredAssetKey;
        private readonly ScanResponse _responseItemContainsTenuredAssetKey;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTenuredAssetKeyWithPropIsNull;
        private readonly ScanResponse _responseItemContainsTenuredAssetKeyWithPropIsNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainTenuredAssetKey;
        private readonly ScanResponse _responseTenuredAssetDoesntContainTenuredAssetKey;


        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNotNull;
        private readonly ScanResponse _responseItemContainsTenureTypeKeyWithPropsAreNotNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNull;
        private readonly ScanResponse _responseItemContainsTenureTypeKeyWithPropsAreNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainTenureTypeKey;
        private readonly ScanResponse _responseItemDoesntContainTenureTypeKey;


        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTerminatedKeyWithPropsAreNotNull;
        private readonly ScanResponse _responseItemContainsTerminatedKeyWithPropsAreNotNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsTerminatedKeyWithPropsAreNull;
        private readonly ScanResponse _responseItemContainsTerminatedKeyWithPropsAreNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainTerminatedKey;
        private readonly ScanResponse _responseItemDoesntContainsTerminatedKey;


        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsPaymentReferenceKey;
        private readonly ScanResponse _responseItemContainsPaymentReferenceKey;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainPaymentReferenceKeyIsNull;
        private readonly ScanResponse _responseItemDoesntContainPaymentReferenceKeyIsNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainPaymentReferenceKey;
        private readonly ScanResponse _responseItemDoesntContainPaymentReferenceKey;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemDoesntContainHouseholdMembersKey;
        private readonly ScanResponse _responseItemDoesntContainHouseholdMembersKeyProps;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsHouseholdMembersKey;
        private readonly ScanResponse _responseItemContainsHouseholdMembersKey;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsHouseholdMembersKeyIsNull;
        private readonly ScanResponse _responseItemContainsHouseholdMembersKeyIsNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNull;
        private readonly ScanResponse _responseItemContainsHouseholdMembersKeyPropsAreNull;

        private readonly IEnumerable<TenureInformation> _expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNotContained;
        private readonly ScanResponse _responseItemContainsHouseholdMembersKeyPropsAreNotContained;

        public ScanResponseExtensionTests()
        {
            _expectedScanResponseItemContainsTenuredAssetKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = new TenuredAsset()
                    {
                        FullAddress = "Address"
                    },
                    TenureType = null,
                    PaymentReference = null,
                    HouseholdMembers = null,
                    //HouseholdMembers = new List<HouseholdMembers>()
                    //{
                    //    new HouseholdMembers()
                    //    {
                    //        Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    //        FullName = "FullName",
                    //        IsResponsible = true
                    //    }
                    //}
                }
            };

            _responseItemContainsTenuredAssetKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "tenuredAsset",
                            new AttributeValue()
                            {
                                S = "tenuredAsset",
                                M = new Dictionary<string, AttributeValue>()
                                {
                                    {
                                        "fullAddress",
                                        new AttributeValue()
                                        {
                                            S = "Address"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _expectedScanResponseItemContainsTenuredAssetKeyWithPropIsNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = new TenuredAsset()
                    {
                        FullAddress = null
                    },
                    TenureType = null,
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsTenuredAssetKeyWithPropIsNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "tenuredAsset",
                            new AttributeValue()
                            {
                                S = "tenuredAsset",
                                M = new Dictionary<string, AttributeValue>()
                                {
                                    {
                                        "fullAddress",
                                        new AttributeValue()
                                        {
                                            S = null
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _expectedScanResponseItemDoesntContainTenuredAssetKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseTenuredAssetDoesntContainTenuredAssetKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } }
                    }
                }
            };

            _expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNotNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = new TenureType()
                    {
                        Code = "Code",
                        Description = "Description"
                    },
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsTenureTypeKeyWithPropsAreNotNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "tenureType",
                            new AttributeValue()
                            {
                                M = new Dictionary<string, AttributeValue>()
                                {
                                    {
                                        "code",
                                        new AttributeValue()
                                        {
                                            S = "Code"
                                        }
                                    },
                                    {
                                        "description",
                                        new AttributeValue()
                                        {
                                            S = "Description"
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            };

            _expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = new TenureType()
                    {
                        Code = null,
                        Description = null
                    },
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsTenureTypeKeyWithPropsAreNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "tenureType",
                            new AttributeValue()
                            {
                                M = new Dictionary<string, AttributeValue>()
                                {
                                    {
                                        "code",
                                        new AttributeValue()
                                        {
                                            S = null
                                        }
                                    },
                                    {
                                        "description",
                                        new AttributeValue()
                                        {
                                            S = null
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            };

            _expectedScanResponseItemDoesntContainTenureTypeKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemDoesntContainTenureTypeKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } }
                    }
                }
            };

            _expectedScanResponseItemContainsTerminatedKeyWithPropsAreNotNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = new Terminated()
                    {
                        IsTerminated = true,
                        ReasonForTermination = "ReasonForTermination"
                    },
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsTerminatedKeyWithPropsAreNotNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "terminated",
                            new AttributeValue()
                            {
                                M = new Dictionary<string, AttributeValue>()
                                {
                                    {
                                        "isTerminated",
                                        new AttributeValue()
                                        {
                                            BOOL = true
                                        }
                                    },
                                    {
                                        "reasonForTermination",
                                        new AttributeValue()
                                        {
                                            S = "ReasonForTermination"
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            };

            _expectedScanResponseItemContainsTerminatedKeyWithPropsAreNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = new Terminated()
                    {
                        IsTerminated = false,
                        ReasonForTermination = null
                    },
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsTerminatedKeyWithPropsAreNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "terminated",
                            new AttributeValue()
                            {
                                M = new Dictionary<string, AttributeValue>()
                                {

                                }
                            }
                        }
                    }

                }
            };

            _expectedScanResponseItemDoesntContainTerminatedKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemDoesntContainsTerminatedKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } }
                    }

                }
            };

            _expectedScanResponseItemContainsPaymentReferenceKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    PaymentReference = "PaymentReference",
                    HouseholdMembers = null
                }
            };

            _responseItemContainsPaymentReferenceKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        { "paymentReference", new AttributeValue() { S = "PaymentReference" } }
                    }

                }
            };

            _expectedScanResponseItemDoesntContainPaymentReferenceKeyIsNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    PaymentReference = null,
                    HouseholdMembers = null
                }
            };

            _responseItemDoesntContainPaymentReferenceKeyIsNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        { "paymentReference", new AttributeValue() { S = null } }
                    }

                }
            };

            _expectedScanResponseItemDoesntContainPaymentReferenceKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = null
                }
            };

            _responseItemDoesntContainPaymentReferenceKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } }
                    }

                }
            };

            _expectedScanResponseItemDoesntContainHouseholdMembersKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = null
                }
            };

            _responseItemDoesntContainHouseholdMembersKeyProps = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } }
                    }

                }
            };

            _expectedScanResponseItemContainsHouseholdMembersKey = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = new List<HouseholdMembers>()
                    {
                        new HouseholdMembers()
                        {
                            Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                            FullName = "FullName",
                            IsResponsible = true
                        }
                    }
                }
            };

            _responseItemContainsHouseholdMembersKey = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                            "householdMembers",
                            new AttributeValue()
                            {
                                L = new List<AttributeValue>()
                                {
                                    new AttributeValue()
                                    {
                                        M = new Dictionary<string, AttributeValue>()
                                        {
                                            {
                                                "id",
                                                new AttributeValue()
                                                {
                                                    S = "54b886f6-3970-49ab-9d96-b357015f9a48"
                                                }
                                            },
                                            {
                                                "fullName",
                                                new AttributeValue()
                                                {
                                                    S = "FullName"
                                                }
                                            },
                                            {
                                                "isResponsible",
                                                new AttributeValue()
                                                {
                                                    BOOL = true
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _expectedScanResponseItemContainsHouseholdMembersKeyIsNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = null
                }
            };

            _responseItemContainsHouseholdMembersKeyIsNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                           "householdMembers",
                            new AttributeValue()
                            {
                                NULL = true
                            //    M = null
                                //L = null
                                //L = new List<AttributeValue>()
                                //{
                                //    new AttributeValue()
                                //    {
                                //        M = null
                                //        M = new Dictionary<string, AttributeValue>()
                                //        {
                                //            {
                                //                "id",
                                //                new AttributeValue()
                                //                {
                                //                    S = null
                                //                }
                                //            },
                                //            {
                                //                "fullName",
                                //                new AttributeValue()
                                //                {
                                //                    S = null
                                //                }
                                //            },
                                //            {
                                //                "isResponsible",
                                //                new AttributeValue()
                                //                {
                                //                    M = null
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                }
            };

            _expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNull = new List<TenureInformation>
            {
                new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = new List<HouseholdMembers>()
                    {
                        new HouseholdMembers()
                        {
                            Id = Guid.Empty,
                            FullName = null,
                            IsResponsible = false
                        }
                    }
                }
            };

            _responseItemContainsHouseholdMembersKeyPropsAreNull = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                           "householdMembers",
                            new AttributeValue()
                            {
                                L = new List<AttributeValue>()
                                {
                                    new AttributeValue()
                                    {
                                        M = new Dictionary<string, AttributeValue>()
                                        {
                                            {
                                                "id",
                                                new AttributeValue()
                                                {
                                                    NULL = true
                                                }
                                            },
                                            {
                                                "fullName",
                                                new AttributeValue()
                                                {
                                                    NULL = true
                                                }
                                            },
                                            {
                                                "isResponsible",
                                                new AttributeValue()
                                                {
                                                    NULL = true
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNotContained = new List<TenureInformation>()
            {
                 new TenureInformation()
                {
                    Id = new Guid("54b886f6-3970-49ab-9d96-b357015f9a48"),
                    TenuredAsset = null,
                    TenureType = null,
                    Terminated = null,
                    HouseholdMembers = new List<HouseholdMembers>()
                    {
                        new HouseholdMembers()
                        {
                            Id = Guid.Empty,
                            FullName = null,
                            IsResponsible = false
                        }
                    }
                }
            };

            _responseItemContainsHouseholdMembersKeyPropsAreNotContained = new ScanResponse()
            {
                Items = new List<Dictionary<string, AttributeValue>>()
                {
                    new Dictionary<string, AttributeValue>()
                    {
                        { "id", new AttributeValue() { S = "54b886f6-3970-49ab-9d96-b357015f9a48" } },
                        {
                           "householdMembers",
                            new AttributeValue()
                            {
                                L = new List<AttributeValue>()
                                {
                                    new AttributeValue()
                                    {
                                        M = new Dictionary<string, AttributeValue>()
                                        {
                                            {
                                                "id",
                                                new AttributeValue()
                                                {
                                                    NULL = true
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void ScanResponseItemContainsTenuredAssetKeyPropIsNotNull()
        {
            var result = _responseItemContainsTenuredAssetKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTenuredAssetKey);
        }

        [Fact]
        public void ScanResponseItemContainsTenuredAssetKeyPropIsNull()
        {
            var result = _responseItemContainsTenuredAssetKeyWithPropIsNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTenuredAssetKeyWithPropIsNull);
        }

        [Fact]
        public void ScanResponseItemDosntContainTenuredAssetKey()
        {
            var result = _responseTenuredAssetDoesntContainTenuredAssetKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainTenuredAssetKey);
        }

        [Fact]
        public void ScanResponseItemContainsTenureTypeKeyPropsAreNotNull()
        {
            var result = _responseItemContainsTenureTypeKeyWithPropsAreNotNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNotNull);
        }

        [Fact]
        public void ScanResponseItemContainsTenureTypeKeyPropsAreNull()
        {
            var result = _responseItemContainsTenureTypeKeyWithPropsAreNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTenureTypeKeyWithPropsAreNull);
        }

        [Fact]
        public void ScanResponseItemDoesntContainTenureTypeKey()
        {
            var result = _responseItemDoesntContainTenureTypeKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainTenureTypeKey);
        }

        [Fact]
        public void ScanResponseItemContainTerminatedKeyPropsAreNotNull()
        {
            var result = _responseItemContainsTerminatedKeyWithPropsAreNotNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTerminatedKeyWithPropsAreNotNull);
        }

        [Fact]
        public void ScanResponseItemContainTerminatedKeyPropsAreNull()
        {
            var result = _responseItemContainsTerminatedKeyWithPropsAreNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsTerminatedKeyWithPropsAreNull);
        }

        [Fact]
        public void ScanResponseItemDoesntContainTerminatedKey()
        {
            var result = _responseItemDoesntContainsTerminatedKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainTerminatedKey);
        }

        [Fact]
        public void ScanResponseItemContainsPaymentReferenceKey()
        {
            var result = _responseItemContainsPaymentReferenceKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsPaymentReferenceKey);
        }

        [Fact]
        public void ScanResponseItemContainsPaymentReferenceKeyIsNull()
        {
            var result = _responseItemDoesntContainPaymentReferenceKeyIsNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainPaymentReferenceKeyIsNull);
        }

        [Fact]
        public void ScanResponseItemDoesntContainsPaymentReferenceKey()
        {
            var result = _responseItemDoesntContainPaymentReferenceKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainPaymentReferenceKey);
        }

        [Fact]
        public void ScanResponseItemDoesntContainHouseholdMembersKey()
        {
            var result = _responseItemDoesntContainHouseholdMembersKeyProps.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemDoesntContainHouseholdMembersKey);
        }

        [Fact]
        public void ScanResponseItemContainsHouseholdMembersKey()
        {
            var result = _responseItemContainsHouseholdMembersKey.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsHouseholdMembersKey);
        }

        [Fact]
        public void ScanResponseItemContainsHouseholdMembersKeyIsNull()
        {
            var result = _responseItemContainsHouseholdMembersKeyIsNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsHouseholdMembersKeyIsNull);
        }

        [Fact]
        public void ScanResponseItemContainsHouseholdMembersKeyPropsAreNull()
        {
            var result = _responseItemContainsHouseholdMembersKeyPropsAreNull.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNull);
        }

        [Fact]
        public void ScanResponseItemContainsHouseholdMembersKeyPropsAreNotContained()
        {
            var result = _responseItemContainsHouseholdMembersKeyPropsAreNotContained.ToTenureInformation();

            result.Should().BeEquivalentTo(_expectedScanResponseItemContainsHouseholdMembersKeyPropsAreNotContained);
        }
    }
}
