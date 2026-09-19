using Moq;
using ProvaPub.Application.Payments;
using Xunit;

namespace ProvaPub.Tests
{
    public class PaymentStrategyResolverTests
    {
        private static IPaymentStrategy CreateStrategy(string paymentMethod)
        {
            var strategy = new Mock<IPaymentStrategy>();
            strategy.Setup(s => s.PaymentMethod).Returns(paymentMethod);
            return strategy.Object;
        }

        [Theory]
        [InlineData("pix", "pix")]
        [InlineData("PIX", "pix")]
        [InlineData("CreditCard", "creditcard")]
        public void Resolve_MatchesRegisteredStrategyIgnoringCase(string requested, string registered)
        {
            var strategies = new[] { CreateStrategy(registered) };
            var sut = new PaymentStrategyResolver(strategies);

            var result = sut.Resolve(requested);

            Assert.Equal(registered, result.PaymentMethod);
        }

        [Fact]
        public void Resolve_UnknownPaymentMethod_ThrowsArgumentException()
        {
            var strategies = new[] { CreateStrategy("pix") };
            var sut = new PaymentStrategyResolver(strategies);

            Assert.Throws<ArgumentException>(() => sut.Resolve("bitcoin"));
        }
    }
}
