using Microsoft.AspNetCore.Mvc;
using RegionalData.Controllers.Dto;
using RegionalData.Core.Interface.Service;

namespace RegionalData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactService _service;

        public ContactController(ILogger<ContactController> logger, IContactService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<ContactDto>>> GetAll()
        {
            var contatos = new List<ContactDto>();
            var contatosEntity = await _service.GetAllAsync();

            if (contatosEntity == null || !contatosEntity.Any())
            {
                return NoContent();
            }
            else
            {
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