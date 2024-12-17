using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RegionalData.Controllers.Dto;
using RegionalData.Core.Interface.Service;

namespace RegionalData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private const string CacheKey = "Contact";
        private readonly ILogger<ContactController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IContactService _service;

        public ContactController(ILogger<ContactController> logger, IMemoryCache memoryCache, IContactService service)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _service = service;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<ContactDto>>> GetAll()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out List<ContactDto> contatos))
            {
                var contatosEntity = await _service.GetAllAsync();

                if (contatosEntity == null || !contatosEntity.Any())
                {
                    return NoContent();
                }
                else
                {
                    contatos = new();

                    foreach (var c in contatosEntity)
                    {
                        contatos.Add(new ContactDto()
                        {
                            Id = c.Id,
                            Email = c.Email,
                            DDD = c.DDD,
                            Cidade = c.Cidade,
                            Estado = c.Estado,
                            Nome = c.Nome,
                            Telefone = c.Telefone
                        });
                    }
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(CacheKey, contatos, cacheOptions);
            }

            return Ok(contatos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> Get(int id)
        {
            var contatoEntity = await _service.GetAsync(id);

            if (contatoEntity == null)
            {
                return NoContent();
            }
            else
            {
                var contact = new ContactDto()
                {
                    Id = contatoEntity.Id,
                    Email = contatoEntity.Email,
                    DDD = contatoEntity.DDD,
                    Cidade = contatoEntity.Cidade,
                    Estado = contatoEntity.Estado,
                    Nome = contatoEntity.Nome,
                    Telefone = contatoEntity.Telefone
                };
                return Ok(contact);
            }
        }

        [HttpGet("ByEmail/{email}")]
        public async Task<ActionResult<List<ContactDto>>> ByEmail(string email)
        {
            var contatosEntity = await _service.GetByEmail(email);

            if (contatosEntity == null || !contatosEntity.Any())
            {
                return NoContent();
            }
            else
            {
                var contatos = new List<ContactDto>();

                foreach (var c in contatosEntity)
                {
                    contatos.Add(new ContactDto()
                    {
                        Id = c.Id,
                        Email = c.Email,
                        DDD = c.DDD,
                        Cidade = c.Cidade,
                        Estado = c.Estado,
                        Nome = c.Nome,
                        Telefone = c.Telefone
                    });
                }
                return Ok(contatos);
            }
        }
    }
}