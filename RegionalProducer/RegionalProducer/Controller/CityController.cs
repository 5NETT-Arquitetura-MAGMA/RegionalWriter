using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RegionalProducer.Controller.Dto;

namespace RegionalProducer.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<CityDto>>> GetAll()
        {
            try
            {
                var client = new HttpClient();
                var url = _configuration["Data:City"];
                if (!string.IsNullOrEmpty(url))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, "City"));
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var cidadesStr = await response.Content.ReadAsStringAsync();
                        var cidades = JsonConvert.DeserializeObject<List<CityDto>>(cidadesStr);

                        return Ok(cidades);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return StatusCode(500, "Falha ao buscar cidades");
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> Get(int id)
        {
            try
            {
                var client = new HttpClient();
                var url = _configuration["Data:City"];
                if (!string.IsNullOrEmpty(url))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, $"/City/{id}"));
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var cidadesStr = await response.Content.ReadAsStringAsync();
                        var cidades = JsonConvert.DeserializeObject<CityDto>(cidadesStr);

                        return Ok(cidades);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return StatusCode(500, "Falha ao buscar cidades");
            }
            return NotFound();
        }

        [HttpGet("ByDDD/{ddd}")]
        public async Task<ActionResult<CityDto>> ByDDD(int ddd)
        {
            try
            {
                var client = new HttpClient();
                var url = _configuration["Data:City"];
                if (!string.IsNullOrEmpty(url))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, Flurl.Url.Combine(url, $"/City/ByDDD/{ddd}"));
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var cidadesStr = await response.Content.ReadAsStringAsync();
                        var cidades = JsonConvert.DeserializeObject<CityDto>(cidadesStr);

                        return Ok(cidades);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return StatusCode(500, "Falha ao buscar cidades");
            }
            return NotFound();
        }
    }
}