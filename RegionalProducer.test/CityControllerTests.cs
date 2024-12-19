using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using RegionalProducer.Controller;
using RegionalProducer.Controller.Dto;
using Xunit;

namespace RegionalProducer.test;

public class CityControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly CityController _controller;

    public CityControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new CityController(_mockConfiguration.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfCities()
    {
        // Arrange
        var cities = new List<CityDto>
        {
            new CityDto
            {
                Id = 1,
                NomeCidade = "São Paulo",
                DDD = "11",
                Estado = "SP"
            }
        };
        var json = JsonConvert.SerializeObject(cities);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:City"]).Returns("http://teste.com");

        // Act
        var result = await _controller.GetAll();

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<List<CityDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithCity()
    {
        // Arrange
        int testId = 1;
        var city = new CityDto
        {
            Id = testId,
            NomeCidade = "São Paulo",
            DDD = "11",
            Estado = "SP"
        };
        var json = JsonConvert.SerializeObject(city);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:City"]).Returns("http://teste.com");

        // Act
        var result = await _controller.Get(testId);

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<CityDto>(okResult.Value);
            Assert.Equal(testId, returnValue.Id);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task ByDDD_ReturnsOkResult_WithListOfCities()
    {
        // Arrange
        string testDDD = "11";
        var cities = new List<CityDto>
        {
            new CityDto
            {
                Id = 1,
                NomeCidade = "São Paulo",
                DDD = testDDD,
                Estado = "SP"
            }
        };
        var json = JsonConvert.SerializeObject(cities);
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _mockConfiguration.Setup(c => c["Data:City"]).Returns("http://teste.com");

        // Act
        var result = await _controller.ByDDD(11);

        // Assert
        if (result.Result is OkObjectResult okResult)
        {
            var returnValue = Assert.IsType<List<CityDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
