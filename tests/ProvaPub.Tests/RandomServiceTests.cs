using FluentAssertions;
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
            // Arrange
            int? acceptedNumber = null;
            var repository = new Mock<IRandomNumberRepository>();
            repository.Setup(r => r.TryAddAsync(It.IsAny<int>()))
                .Callback<int>(n => acceptedNumber = n)
                .ReturnsAsync(true);
            var sut = new RandomService(repository.Object);

            // Act
            var result = await sut.GetRandom();

            // Assert
            result.Should().Be(acceptedNumber);
            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetRandom_RepositoryRejectsFirstAttempts_RetriesUntilAccepted()
        {
            // Arrange
            var repository = new Mock<IRandomNumberRepository>();
            repository.SetupSequence(r => r.TryAddAsync(It.IsAny<int>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(true);
            var sut = new RandomService(repository.Object);

            // Act
            await sut.GetRandom();

            // Assert
            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetRandom_RepositoryAlwaysRejects_ThrowsAfterMaxAttempts()
        {
            // Arrange
            var repository = new Mock<IRandomNumberRepository>();
            repository.Setup(r => r.TryAddAsync(It.IsAny<int>())).ReturnsAsync(false);
            var sut = new RandomService(repository.Object);

            // Act
            var act = () => sut.GetRandom();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            repository.Verify(r => r.TryAddAsync(It.IsAny<int>()), Times.Exactly(5));
        }
    }
}
