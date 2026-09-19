using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Validators;
using Xunit;

namespace ProvaPub.Tests
{
    public class OrderRequestValidatorTests
    {
        private readonly OrderRequestValidator _sut = new();

        [Fact]
        public void Validate_ValidRequest_HasNoErrors()
        {
            var result = _sut.Validate(new OrderRequest("pix", 150m, 7));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_CustomerIdNotGreaterThanZero_FailsWithExpectedMessage(int customerId)
        {
            var result = _sut.Validate(new OrderRequest("pix", 150m, customerId));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CustomerIdMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_PaymentValueNotGreaterThanZero_FailsWithExpectedMessage(decimal paymentValue)
        {
            var result = _sut.Validate(new OrderRequest("pix", paymentValue, 7));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PaymentValueMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData("p")]
        [InlineData("pi")]
        public void Validate_PaymentMethodTooShort_FailsWithExpectedMessage(string paymentMethod)
        {
            var result = _sut.Validate(new OrderRequest(paymentMethod, 150m, 7));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PaymentMethodTooShort);
        }

        [Fact]
        public void Validate_PaymentMethodEmpty_FailsWithRequiredMessage()
        {
            var result = _sut.Validate(new OrderRequest("", 150m, 7));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PaymentMethodRequired);
        }

        [Fact]
        public void Validate_PaymentMethodNull_FailsWithRequiredMessage()
        {
            var result = _sut.Validate(new OrderRequest(null!, 150m, 7));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PaymentMethodRequired);
        }
    }
}
