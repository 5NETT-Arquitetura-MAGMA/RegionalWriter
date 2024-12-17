using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RegionalProducer.Controller.Dto;
using System.Text;
using System.Text.Json;

namespace RegionalProducer.Controller;

[ApiController]
[Route("[controller]")]
public class RegionalController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IBus _bus;

    public RegionalController(IConfiguration configuration, ISendEndpointProvider sendEndpointProvider, IBus bus)
    {
        _configuration = configuration;
        _sendEndpointProvider = sendEndpointProvider;
        _bus = bus;
    }

    [HttpPost]
    [Route("/Contact")]
    public async Task<IActionResult> Post([FromBody] PostContactDto contact)
    {
        try
        {
            if (contact == null)
            {
                return BadRequest("Dados inválidos.");
            }

            string rabbitUser = _configuration["Queue:User"] ?? "guest";
            string rabbitPassword = _configuration["Queue:Password"] ?? "guest";
            string rabbitHost = _configuration["Queue:Host"] ?? "localhost";

            var factory = new ConnectionFactory()
            {
                HostName = rabbitHost,
                UserName = rabbitUser,
                Password = rabbitPassword
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync("regional_create", true, false, false, null);

            var message = JsonSerializer.Serialize(contact);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_create", body: body);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao criar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
    }

    [HttpPut]
    [Route("/Contact")]
    public async Task<IActionResult> Update([FromBody] UpdateContactDto contact)
    {
        try
        {
            string rabbitUser = _configuration["Queue:User"] ?? "guest";
            string rabbitPassword = _configuration["Queue:Password"] ?? "guest";
            string rabbitHost = _configuration["Queue:Host"] ?? "localhost";

            var factory = new ConnectionFactory()
            {
                HostName = rabbitHost,
                UserName = rabbitUser,
                Password = rabbitPassword
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync("regional_update", true, false, false, null);
            var message = JsonSerializer.Serialize(contact);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_update", body: body);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao atualizar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
    }

    [HttpDelete]
    [Route("/Contact")]
    public async Task<IActionResult> Delete([FromBody] int id)
    {
        try
        {
            string rabbitUser = _configuration["Queue:User"] ?? "guest";
            string rabbitPassword = _configuration["Queue:Password"] ?? "guest";
            string rabbitHost = _configuration["Queue:Host"] ?? "localhost";

            var factory = new ConnectionFactory()
            {
                HostName = rabbitHost,
                UserName = rabbitUser,
                Password = rabbitPassword
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync("regional_delete", true, false, false, null);
            var message = JsonSerializer.Serialize(new { id });
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_delete", body: body);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao remover contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
    }
}