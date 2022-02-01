using FluentAssertions;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Xunit;

namespace FinanceDataMigrationApi.Tests
{
    public class EnumExtensionsTests
    {
        [Theory]
        [InlineData(TransactionType.GroundsMaintenance, "Grounds Maintenance")]
        [InlineData(TransactionType.BasicRentNoVAT, "Basic Rent (No VAT)")]
        [InlineData(TransactionType.DebitOrCreditCard, "Debit / Credit Card")]
        public void GetEnumValueAsStringFromDescription(TransactionType transactionType, string description)
        {
            var enumValue = EnumExtensions.GetValueFromDescription<TransactionType>(description);

            enumValue.ToString().Should().Be(transactionType.ToString());
        }

        [Fact]
        public void GetTransactionTypeEnumValueFromDescription()
        {
            var enumValue = EnumExtensions.GetValueFromDescription<TransactionType>("Debit / Credit Card");

            enumValue.Should().Be(TransactionType.DebitOrCreditCard);

        }

        [Fact]
        public void GetTransactionTypeEnumValueAsStringFromDescription()
        {
            var enumValue = EnumExtensions.GetValueFromDescription<TransactionType>("Debit / Credit Card");

            enumValue.ToString().Should().Be("DebitOrCreditCard");
        }


    }
}
