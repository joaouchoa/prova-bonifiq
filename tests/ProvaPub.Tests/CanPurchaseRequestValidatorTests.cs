using FluentAssertions;
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
            // Arrange
            var request = new CanPurchaseRequest(7, 50m);

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
            var request = new CanPurchaseRequest(customerId, 50m);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.CustomerIdMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_PurchaseValueNotGreaterThanZero_FailsWithExpectedMessage(decimal purchaseValue)
        {
            // Arrange
            var request = new CanPurchaseRequest(7, purchaseValue);

            // Act
            var result = _sut.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == ValidationMessages.ValueMustBeGreaterThanZero);
        }
    }
}
