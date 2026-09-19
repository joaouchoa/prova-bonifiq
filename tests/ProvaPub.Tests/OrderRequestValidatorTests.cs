using FluentAssertions;
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
            // Arrange
            var request = new OrderRequest("pix", 150m, 7);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_CustomerIdNotGreaterThanZero_FailsWithExpectedMessage(int customerId)
        {
            // Arrange
            var request = new OrderRequest("pix", 150m, customerId);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.CustomerIdMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_PaymentValueNotGreaterThanZero_FailsWithExpectedMessage(decimal paymentValue)
        {
            // Arrange
            var request = new OrderRequest("pix", paymentValue, 7);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.ValueMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData("p")]
        [InlineData("pi")]
        public void Validate_PaymentMethodTooShort_FailsWithExpectedMessage(string paymentMethod)
        {
            // Arrange
            var request = new OrderRequest(paymentMethod, 150m, 7);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.PaymentMethodTooShort);
        }

        [Fact]
        public void Validate_PaymentMethodEmpty_FailsWithRequiredMessage()
        {
            // Arrange
            var request = new OrderRequest("", 150m, 7);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.PaymentMethodRequired);
        }

        [Fact]
        public void Validate_PaymentMethodNull_FailsWithRequiredMessage()
        {
            // Arrange
            var request = new OrderRequest(null!, 150m, 7);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.PaymentMethodRequired);
        }
    }
}
