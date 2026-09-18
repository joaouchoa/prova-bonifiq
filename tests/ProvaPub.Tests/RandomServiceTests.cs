using Moq;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Services;
using Xunit;

namespace ProvaPub.Tests
{
    public class RandomServiceTests
    {
        [Fact]
        public async Task GetRandom_RepositoryAcceptsFirstTry_ReturnsTheAcceptedNumber()
        {
            int? acceptedNumber = null;
            var repository = new Mock<IRandomNumberRepository>();
            repository.Setup(r => r.TryAddAsync(It.IsAny<int>()))
                .Callback<int>(n => acceptedNumber = n)
                .ReturnsAsync(true);
            var sut = new RandomService(repository.Object);

            var result = await sut.GetRandom();

            Assert.Equal(acceptedNumber, result);
            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetRandom_RepositoryRejectsFirstAttempts_RetriesUntilAccepted()
        {
            var repository = new Mock<IRandomNumberRepository>();
            repository.SetupSequence(r => r.TryAddAsync(It.IsAny<int>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(true);
            var sut = new RandomService(repository.Object);

            await sut.GetRandom();

            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetRandom_RepositoryAlwaysRejects_ThrowsAfterMaxAttempts()
        {
            var repository = new Mock<IRandomNumberRepository>();
            repository.Setup(r => r.TryAddAsync(It.IsAny<int>())).ReturnsAsync(false);
            var sut = new RandomService(repository.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetRandom());
            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Exactly(5));
        }
    }
}
