using FinanceDataMigrationApi.V1.Infrastructure.Extensions;
using FluentAssertions;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using System;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Infrastructure.Extensions
{
    public class EnumsExtensionsTests
    {
        [Fact]
        public void ToEnumStringValueAccountStatusIsNotNullShouldReturnsEnumValue()
        {
            var expectedResult = AccountStatus.Active;
            var accountStatus = "Active";

            var result = accountStatus.ToEnumValue<AccountStatus>();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToEnumStringValueAccountStatusIsNullShouldReturnsDefaultValue()
        {
            var expectedResult = default(AccountStatus);
            string accountStatus = null;

            var result = accountStatus.ToEnumValue<AccountStatus>();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToEnumStringValueAccountStatusIsUndefinedShouldThrowsException()
        {
            var accountStatus = "UndefinedStatus";

            Action act = () => accountStatus.ToEnumValue<AccountStatus>();

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsGpsShouldReturnsRentGroupTypeGarages()
        {
            var expectedResult = RentGroupType.Garages;
            var rentGroupType = "GPS";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsHGFShouldReturnsRentGroupTypeGenFundRents()
        {
            var expectedResult = RentGroupType.GenFundRents;
            var rentGroupType = "HGF";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsHRAShouldReturnsRentGroupTypeHraRents()
        {
            var expectedResult = RentGroupType.HraRents;
            var rentGroupType = "HRA";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsLMWShouldReturnsRentGroupTypeMajorWorks()
        {
            var expectedResult = RentGroupType.MajorWorks;
            var rentGroupType = "LMW";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsLSCShouldReturnsRentGroupTypeLeaseHolders()
        {
            var expectedResult = RentGroupType.LeaseHolders;
            var rentGroupType = "LSC";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsTAGShouldReturnsRentGroupTypeTempAcc()
        {
            var expectedResult = RentGroupType.TempAcc;
            var rentGroupType = "TAG";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsTRAShouldReturnsRentGroupTypeTravelers()
        {
            var expectedResult = RentGroupType.Travelers;
            var rentGroupType = "TRA";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsTAHShouldReturnsRentGroupTypeTempAccHRA()
        {
            var expectedResult = RentGroupType.TempAccHRA;
            var rentGroupType = "TAH";

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsUndefinedShouldThrowsArgumentException()
        {
            var rentGroupType = "UndefinedRentGroupType";

            Action act = () => rentGroupType.ToRentGroup();

            act.Should().Throw<ArgumentException>().WithMessage("Exception was thrown in ToRentGroup method " +
                            "while attempting converting rent group string representation to enum " +
                            "representation in 'MSSQL to ES' transferring.");
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeIsNullShouldReturnsDefaultRentGroupType()
        {
            var expectedResult = default(RentGroupType);
            string rentGroupType = null;

            var result = rentGroupType.ToRentGroup();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ToRentGroupStringRentGroupTypeHasIncorrectFormShouldThrowsException()
        {
            var accountStatus = string.Empty;

            Action act = () => accountStatus.ToEnumValue<AccountStatus>();

            act.Should().Throw<Exception>();
        }
    }
}
