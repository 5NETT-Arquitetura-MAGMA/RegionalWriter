using CityData.Controllers.Dtos;
using CityData.Core.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CityData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private const string CacheKey = "Cidades";
        private readonly ILogger<CityController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ICityService _service;

        public CityController(ILogger<CityController> logger, IMemoryCache memoryCache, ICityService service)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _service = service;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<CityDto>>> GetAll()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out List<CityDto> cidades))
            {
                var cidadesEntity = await _service.GetAllAsync();

                if (cidadesEntity == null || !cidadesEntity.Any())
                {
                    return NotFound("Cidades não encontradas");
                }
                else
                {
                    cidades = new();

                    foreach (var c in cidadesEntity)
                    {
                        cidades.Add(new CityDto()
                        {
                            DDD = c.DDD,
                            Estado = c.Estado,
                            Id = c.Id,
                            NomeCidade = c.NomeCidade
                        });
                    }
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(CacheKey, cidades, cacheOptions);
            }

            return Ok(cidades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> Get(int id)
        {
            var cidadeEntity = await _service.GetAsync(id);

            if (cidadeEntity == null)
            {
                return NotFound("Cidade não encontrada");
            }
            else
            {
                var cidade = new CityDto()
                {
                    NomeCidade = cidadeEntity.NomeCidade,
                    Id = cidadeEntity.Id,
                    Estado = cidadeEntity.Estado,
                    DDD = cidadeEntity.DDD
                };
                return Ok(cidade);
            }
        }

        [HttpGet("ByDDD/{ddd}")]
        public async Task<ActionResult<List<CityDto>>> GetByDDD(int ddd)
        {
            var cidadesEntity = await _service.GetByDDD(ddd);

            if (cidadesEntity == null)
            {
                return NotFound("Cidade não encontrada");
            }
            else
            {
                var cidades = new List<CityDto>();
                foreach (var c in cidadesEntity)
                {
                    cidades.Add(new CityDto()
                    {
                        NomeCidade = c.NomeCidade,
                        Id = c.Id,
                        Estado = c.Estado,
                        DDD = c.DDD
                    });
                }
                return Ok(cidades);
            }
        }
    }
}