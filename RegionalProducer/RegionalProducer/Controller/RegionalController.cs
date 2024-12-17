using System.Text;
using System.Text.Json;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace RegionalProducer.Controller;

[ApiController]
[Route("[controller]")]
public class RegionalController : ControllerBase
{
    [HttpPost]
    [Route("/Contact")]
    public async Task<IActionResult> Post([FromBody] Contact contact)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "regional",
            Password = "R3gional!234"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync("regional_create", false, false, false, null);
        var message = JsonSerializer.Serialize(contact);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_create", body: body);
        return Ok("Hello World!");
    }
    
    [HttpPut]
    [Route("/Contact")]
    public async Task<IActionResult> Update([FromBody] Contact contact)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "regional",
            Password = "R3gional!234"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync("regional_update", false, false, false, null);
        var message = JsonSerializer.Serialize(contact);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_update", body: body);
        return Ok("Hello World!");
    }
    
    [HttpDelete]
    [Route("/Contact")]
    public async Task<IActionResult> Delete([FromBody] int id)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "regional",
            Password = "R3gional!234"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync("regional_delete", false, false, false, null);
        var message = JsonSerializer.Serialize(new { id});
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_delete", body: body);
        return Ok("Hello World!");
    }
}