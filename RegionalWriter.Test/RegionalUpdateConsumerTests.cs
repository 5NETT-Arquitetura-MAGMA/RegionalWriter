using Moq;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using Xunit;
using System;
using System.Threading.Tasks;

namespace RegionalWriter.Test
{
    public class RegionalUpdateConsumerTests
    {
        private readonly Mock<IConnectionStringProvider> _mockConnectionStringProvider;
        private readonly RegionalUpdateConsumer _consumer;

        public RegionalUpdateConsumerTests()
        {
            _mockConnectionStringProvider = new Mock<IConnectionStringProvider>();
            _consumer = new RegionalUpdateConsumer(_mockConnectionStringProvider.Object);
        }

        [Fact]
        public async Task Execute_ShouldUpdateContact_WhenDtoIsValid()
        {
            // Arrange
            var dto = new RegionalUpdateDto
            {
                Id = 1,
                Nome = "Nome",
                Telefone = 123456789,
                DDD = 11,
                Email = "email@example.com",
                Estado = "SP",
                Cidade = "São Paulo"
            };

            var contact = new Contact { Id = dto.Id };

            _mockConnectionStringProvider.Setup(c => c.GetContactAsync(dto.Id)).ReturnsAsync(contact);

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.UpdateContactAsync(It.Is<Contact>(ct => ct.Id == dto.Id)), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldNotUpdateContact_WhenDtoIsInvalid()
        {
            // Arrange
            var dto = new RegionalUpdateDto
            {
                Id = 0
            };

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.UpdateContactAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task Execute_ShouldHandleException_WhenErrorOccurs()
        {
            // Arrange
            var dto = new RegionalUpdateDto
            {
                Id = 1,
                Nome = "Nome",
                Telefone = 123456789,
                DDD = 11,
                Email = "email@example.com",
                Estado = "SP",
                Cidade = "São Paulo"
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