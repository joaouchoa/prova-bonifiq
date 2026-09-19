using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Validators;
using Xunit;

namespace ProvaPub.Tests
{
    public class CanPurchaseRequestValidatorTests
    {
        private readonly CanPurchaseRequestValidator _sut = new();

        [Fact]
        public void Validate_ValidRequest_HasNoErrors()
        {
            var result = _sut.Validate(new CanPurchaseRequest(7, 50m));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_CustomerIdNotGreaterThanZero_FailsWithExpectedMessage(int customerId)
        {
            var result = _sut.Validate(new CanPurchaseRequest(customerId, 50m));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CustomerIdMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_PurchaseValueNotGreaterThanZero_FailsWithExpectedMessage(decimal purchaseValue)
        {
            var result = _sut.Validate(new CanPurchaseRequest(7, purchaseValue));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ValueMustBeGreaterThanZero);
        }
    }
}
