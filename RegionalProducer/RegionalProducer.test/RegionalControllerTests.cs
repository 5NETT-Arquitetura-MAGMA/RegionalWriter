using System.Net;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using RegionalProducer.Controller;
using RegionalProducer.Controller.Dto;
using Xunit;

namespace RegionalProducer.test;

public class RegionalControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ISendEndpointProvider> _mockSendEndpointProvider;
    private readonly Mock<IBus> _mockBus;
    private readonly RegionalController _controller;

    public RegionalControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockSendEndpointProvider = new Mock<ISendEndpointProvider>();
        _mockBus = new Mock<IBus>();
        _controller = new RegionalController(_mockConfiguration.Object, _mockSendEndpointProvider.Object, _mockBus.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfContacts()
    {
        // Arrange
        var contacts = new List<ContactDto>
        {
            new ContactDto
            {
                Id = 1,
                Nome = "Test",
                Telefone = 123456789,
                DDD = 12,
                Email = "test@example.com",
                Estado = "SP",
                Cidade = "São Paulo"
            }
        };
        var json = JsonConvert.SerializeObject(contacts);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:Contacts"]).Returns("http://teste.com");

        // Act
        var result = await _controller.GetAll();

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<List<ContactDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithContact()
    {
        // Arrange
        int testId = 1;
        var contact = new ContactDto
        {
            Id = testId,
            Nome = "Test",
            Telefone = 123456789,
            DDD = 12,
            Email = "test@example.com",
            Estado = "SP",
            Cidade = "São Paulo"
        };
        var json = JsonConvert.SerializeObject(contact);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:Contacts"]).Returns("http://teste.com");

        // Act
        var result = await _controller.Get(testId);

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<ContactDto>(okResult.Value);
            Assert.Equal(testId, returnValue.Id);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task ByEmail_ReturnsOkResult_WithListOfContacts()
    {
        // Arrange
        string testEmail = "test@example.com";
        var contacts = new List<ContactDto>
        {
            new ContactDto
            {
                Id = 1,
                Nome = "Test",
                Telefone = 123456789,
                DDD = 12,
                Email = testEmail,
                Estado = "SP",
                Cidade = "São Paulo"
            }
        };
        var json = JsonConvert.SerializeObject(contacts);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:Contacts"]).Returns("http://teste.com");

        // Act
        var result = await _controller.ByEmail(testEmail);

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<List<ContactDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task Create_ReturnsOkObjectResult()
    {
        // Arrange
        var newContact = new PostContactDto
        {
            Nome = "New Contact",
            Telefone = 123456789,
            DDD = 12,
            Email = "newcontact@example.com",
            Estado = "SP",
            Cidade = "São Paulo"
        };
        var json = JsonConvert.SerializeObject(newContact);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:City"]).Returns("http://teste.com");

        // Act
        var result = await _controller.Post(newContact);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult()
    {
        // Arrange
        var updatedContact = new UpdateContactDto
        {
            Id = 1,
            Nome = "Updated Contact",
            Telefone = 987654321,
            DDD = 21,
            Email = "updatedcontact@example.com",
            Estado = "RJ",
            Cidade = "Rio de Janeiro"
        };
        var json = JsonConvert.SerializeObject(updatedContact);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:City"]).Returns("http://teste.com");

        // Act
        var result = await _controller.Update(updatedContact);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult()
    {
        // Arrange
        int testId = 1;
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:Contact"]).Returns($"http://teste.com/{testId}");

        // Act
        var result = await _controller.Delete(testId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
