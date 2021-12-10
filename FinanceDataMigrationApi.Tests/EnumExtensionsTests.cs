using FinanceDataMigrationApi.V1.Domain;
using FluentAssertions;
using Xunit;

namespace FinanceDataMigrationApi.Tests
{
    public class EnumExtensionsTests
    {

        [Theory]
        //[InlineData(TransactionType.Rent, "Rent")]
        //[InlineData(TransactionType.Charge, "Charge")]
        [InlineData(TransactionType.GroundsMaintenance, "Grounds Maintenance")]
        [InlineData(TransactionType.BasicRentNoVAT, "Basic Rent (No VAT)")]
        public void GetDisplayNameReturnsTransactionTypeDisplayAttributeName(TransactionType transactionType, string displayNameValue)
        {
            // Arrange 
            var enumValue = transactionType;

            // Act
            var displayName = enumValue.GetDisplayName();

            // Assert
            displayName.Should().Be(displayNameValue);
        }


    }
}
