using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RegionalProducer.Controller.Dto;
using System.Text;

namespace RegionalProducer.Controller;

[ApiController]
[Route("Contact")]
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

    [HttpGet("")]
    public async Task<ActionResult<List<ContactDto>>> GetAll()
    {
        try
        {
            var client = new HttpClient();
            var url = _configuration["Data:Contacts"];
            if (!string.IsNullOrEmpty(url))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, "Contact"));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var contatosStr = await response.Content.ReadAsStringAsync();
                    var contatos = JsonConvert.DeserializeObject<List<ContactDto>>(contatosStr);

                    return Ok(contatos);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao buscar contatos. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
        return NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> Get(int id)
    {
        try
        {
            try
            {
                var client = new HttpClient();
                var url = _configuration["Data:Contacts"];
                if (!string.IsNullOrEmpty(url))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, $"Contact/{id}"));
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var contatosStr = await response.Content.ReadAsStringAsync();
                        var contato = JsonConvert.DeserializeObject<ContactDto>(contatosStr);

                        return Ok(contato);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

                return Ok(new { success = false });
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao buscar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
    }

    [HttpGet("ByEmail/{email}")]
    public async Task<ActionResult<List<ContactDto>>> ByEmail(string email)
    {
        try
        {
            var client = new HttpClient();
            var url = _configuration["Data:Contacts"];
            if (!string.IsNullOrEmpty(url))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, $"Contact/ByEmail/{Flurl.Url.EncodeIllegalCharacters(email)}"));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var contatosStr = await response.Content.ReadAsStringAsync();
                    var contatos = JsonConvert.DeserializeObject<List<ContactDto>>(contatosStr);

                    return Ok(contatos);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao buscar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
        return NotFound();
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Post([FromBody] PostContactDto contact)
    {
        try
        {
            if (contact == null)
            {
                return BadRequest("Dados inválidos.");
            }
            var client = new HttpClient();

            var cities = new List<CityDto>();
            var url = _configuration["Data:City"];
            if (!string.IsNullOrEmpty(url))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, "City"));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var cidadesStr = await response.Content.ReadAsStringAsync();
                    cities = JsonConvert.DeserializeObject<List<CityDto>>(cidadesStr);
                    if (cities != null && cities.Count > 0)
                    {
                        if (cities.Any(x => x.DDD == contact.DDD.ToString()))
                        {
                            if (cities.Any(x => x.Estado.ToLower() == contact.Estado.ToLower()))
                            {
                                if (cities.Any(x => x.NomeCidade.ToLower() == contact.Cidade.ToLower()))
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
                                    await channel.QueueDeclareAsync("regional_create", true, false, false, null);

                                    var message = JsonConvert.SerializeObject(contact);
                                    var body = Encoding.UTF8.GetBytes(message);
                                    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_create", body: body);

                                    return Ok(new { success = true });
                                }
                                else
                                {
                                    return Ok(new { success = false, message = "Cidade não encontrada" });
                                }
                            }
                            else
                            {
                                return Ok(new { success = false, message = "Estado não encontrado" });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, message = "DDD não encontrado" });
                        }
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Cidades não cadastradas" });
                    }
                }
                else
                {
                    var cidadesStr = await response.Content.ReadAsStringAsync();
                    return Ok(new { success = false, message = cidadesStr });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao criar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false, message = $@"Falha ao criar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}" });
        }
        return Ok(new { success = false });
    }

    [HttpPut]
    [Route("")]
    public async Task<IActionResult> Update([FromBody] UpdateContactDto contact)
    {
        try
        {
            var client = new HttpClient();

            var cities = new List<CityDto>();
            var url = _configuration["Data:City"];
            if (!string.IsNullOrEmpty(url))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, "City"));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var cidadesStr = await response.Content.ReadAsStringAsync();
                    cities = JsonConvert.DeserializeObject<List<CityDto>>(cidadesStr);
                    if (cities != null && cities.Count > 0)
                    {
                        if (cities.Any(x => x.DDD == contact.DDD.ToString()))
                        {
                            if (cities.Any(x => x.Estado.ToLower() == contact.Estado.ToLower()))
                            {
                                if (cities.Any(x => x.NomeCidade.ToLower() == contact.Cidade.ToLower()))
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
                                    var message = JsonConvert.SerializeObject(contact);
                                    var body = Encoding.UTF8.GetBytes(message);
                                    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "regional_update", body: body);
                                    return Ok(new { success = true });
                                }
                                else
                                {
                                    return Ok(new { success = false, message = "Cidade não encontrada" });
                                }
                            }
                            else
                            {
                                return Ok(new { success = false, message = "Estado não encontrado" });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, message = "DDD não encontrado" });
                        }
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Cidades não cadastradas" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Falha ao atualizar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");

            return Ok(new { success = false });
        }
        return Ok(new { success = false });
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
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
            var message = JsonConvert.SerializeObject(new { id });
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