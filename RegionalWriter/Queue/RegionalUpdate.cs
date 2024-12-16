using Dapper.FastCrud;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using System.Data;

namespace RegionalWriter.Queue
{
    public class RegionalUpdateConsumer : IConsumer<RegionalUpdateDto>
    {
        private readonly IConfiguration _configuration;

        public RegionalUpdateConsumer(IConfiguration configuration)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<RegionalUpdateDto> context)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var dto = context.Message;
                    if (
                        dto != null &&
                        !string.IsNullOrEmpty(dto.Nome) &&
                        dto.Telefone > 0 &&
                        dto.DDD > 0 &&
                        !string.IsNullOrEmpty(dto.Email) &&
                        !string.IsNullOrEmpty(dto.Estado) &&
                        !string.IsNullOrEmpty(dto.Cidade) &&
                        dto.Id > 0
                        )
                    {
                        using IDbConnection dbConnection = new SqlConnection(connectionString);
                        var contact = await dbConnection.GetAsync(new Contact { Id = dto.Id });
                        if (contact != null)
                        {
                            contact.Cidade = dto.Cidade;
                            contact.Estado = dto.Estado;
                            contact.Email = dto.Email;
                            contact.DDD = dto.DDD;
                            contact.Nome = dto.Nome;
                            contact.Telefone = dto.Telefone;

                            await dbConnection.UpdateAsync(contact);
                            Console.WriteLine("Registro atualizado com sucesso!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao atualizar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        }
    }
}