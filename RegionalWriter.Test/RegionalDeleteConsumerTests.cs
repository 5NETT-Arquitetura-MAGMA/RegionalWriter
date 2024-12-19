using Moq;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using Xunit;

namespace RegionalWriter.Test
{
    public class RegionalDeleteConsumerTests
    {
        private readonly Mock<IConnectionStringProvider> _mockConnectionStringProvider;
        private readonly RegionalDeleteConsumer _consumer;

        public RegionalDeleteConsumerTests()
        {
            _mockConnectionStringProvider = new Mock<IConnectionStringProvider>();
            _consumer = new RegionalDeleteConsumer(_mockConnectionStringProvider.Object);
        }

        [Fact]
        public async Task Execute_ShouldDeleteContact_WhenDtoIsValid()
        {
            // Arrange
            var dto = new RegionalDeleteDto
            {
                Id = 1
            };

            var contact = new Contact { Id = dto.Id };

            _mockConnectionStringProvider.Setup(c => c.GetContactAsync(dto.Id)).ReturnsAsync(contact);

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.DeleteContactAsync(It.Is<Contact>(ct => ct.Id == dto.Id)), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldNotDeleteContact_WhenDtoIsInvalid()
        {
            // Arrange
            var dto = new RegionalDeleteDto
            {
                Id = 0
            };

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.DeleteContactAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task Execute_ShouldHandleException_WhenErrorOccurs()
        {
            // Arrange
            var dto = new RegionalDeleteDto
            {
                Id = 1
            };

            _mockConnectionStringProvider.Setup(c => c.GetContactAsync(dto.Id)).ThrowsAsync(new Exception("Database error"));

            // Act
            Exception? exception = null;
            try
            {
                await _consumer.Execute(dto);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
            Assert.Equal("Database error", exception.Message);
        }
    }
}
