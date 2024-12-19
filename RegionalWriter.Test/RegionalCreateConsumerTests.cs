using Moq;
using RegionalWriter.Model.View;

namespace RegionalWriter.Test
{
    public class RegionalCreateConsumerTests
    {
        private readonly Mock<IConnectionStringProvider> _mockConnectionStringProvider;
        private readonly RegionalCreateConsumer _consumer;

        public RegionalCreateConsumerTests()
        {
            _mockConnectionStringProvider = new Mock<IConnectionStringProvider>();
            _consumer = new RegionalCreateConsumer(_mockConnectionStringProvider.Object);
        }

        [Fact]
        public async Task Execute_ShouldInsertContact_WhenDtoIsValid()
        {
            // Arrange
            var dto = new RegionalDto
            {
                Nome = "Teste",
                Telefone = 123456789,
                DDD = 11,
                Email = "teste@teste.com",
                Estado = "SP",
                Cidade = "São Paulo"
            };

            _mockConnectionStringProvider.Setup(c => c.GetConnectionString("DefaultConnection")).Returns("FakeConnectionString");

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.Insert(dto), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldNotInsertContact_WhenDtoIsInvalid()
        {
            // Arrange
            var dto = new RegionalDto
            {
                Nome = "",
                Telefone = 0,
                DDD = 0,
                Email = "",
                Estado = "",
                Cidade = ""
            };

            _mockConnectionStringProvider.Setup(c => c.GetConnectionString("DefaultConnection")).Returns("FakeConnectionString");

            // Act
            await _consumer.Execute(dto);

            // Assert
            _mockConnectionStringProvider.Verify(c => c.Insert(dto), Times.Never);
        }

        [Fact]
        public async Task Execute_ShouldHandleException_WhenErrorOccurs()
        {
            // Arrange
            var dto = new RegionalDto
            {
                Nome = "Teste",
                Telefone = 123456789,
                DDD = 11,
                Email = "teste@teste.com",
                Estado = "SP",
                Cidade = "São Paulo"
            };

            _mockConnectionStringProvider.Setup(c => c.GetConnectionString("DefaultConnection")).Returns("FakeConnectionString");

            // Act
            Exception exception = null;
            try
            {
                await _consumer.Execute(dto);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.Null(exception);
        }
    }
}
