using Dapper.FastCrud;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using System.Data;

namespace RegionalWriter.Queue
{
    public class RegionalUpdateConsumer
    {
        private readonly IConfiguration _configuration;

        public RegionalUpdateConsumer(IConfiguration configuration)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _configuration = configuration;
        }

        public async Task Execute(RegionalUpdateDto dto)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    if (
                        dto != null &&
                        dto.Id > 0
                        )
                    {
                        using IDbConnection dbConnection = new SqlConnection(connectionString);
                        var contact = await dbConnection.GetAsync(new Contact { Id = dto.Id });
                        if (contact != null)
                        {
                            if (!string.IsNullOrEmpty(dto.Cidade))
                                contact.Cidade = dto.Cidade;
                            if (!string.IsNullOrEmpty(dto.Estado))
                                contact.Estado = dto.Estado;
                            if (!string.IsNullOrEmpty(dto.Email))
                                contact.Email = dto.Email;
                            if (dto.DDD.HasValue && dto.DDD.Value > 0)
                                contact.DDD = dto.DDD.Value;
                            if (!string.IsNullOrEmpty(dto.Nome))
                                contact.Nome = dto.Nome;
                            if (dto.Telefone.HasValue && dto.Telefone.Value > 0)
                                contact.Telefone = dto.Telefone.Value;

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