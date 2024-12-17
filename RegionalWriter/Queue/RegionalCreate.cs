using Dapper;
using Dapper.FastCrud;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using System.Data;

namespace RegionalWriter.Queue
{
    public class RegionalCreateConsumer
    {
        private readonly IConfiguration _configuration;

        public RegionalCreateConsumer(IConfiguration configuration)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _configuration = configuration;
        }

        public async Task Execute(RegionalDto dto)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    if (
                        dto != null &&
                        !string.IsNullOrEmpty(dto.Nome) &&
                        dto.Telefone > 0 &&
                        dto.DDD > 0 &&
                        !string.IsNullOrEmpty(dto.Email) &&
                        !string.IsNullOrEmpty(dto.Estado) &&
                        !string.IsNullOrEmpty(dto.Cidade)
                        )
                    {
                        using IDbConnection dbConnection = new SqlConnection(connectionString);
                        var contactExisting = (await dbConnection.QueryAsync<Contact>($"SELECT Id, Nome, Telefone, DDD, Email, Estado, Cidade FROM Contacts where Email = '{dto.Email}'")).ToList();
                        if (contactExisting != null && contactExisting.Count > 0)
                        {
                            if (contactExisting != null && contactExisting.Any(x => x.Cidade == dto.Cidade && x.Estado == dto.Estado && x.DDD == dto.DDD && x.Nome == dto.Nome && x.Telefone == dto.Telefone))
                            {
                                Console.WriteLine($"Contato com o email {dto.Email} ja existe");
                                return;
                            }
                            var contact = new Contact()
                            {
                                Cidade = dto.Cidade,
                                Estado = dto.Estado,
                                Email = dto.Email,
                                DDD = dto.DDD,
                                Nome = dto.Nome,
                                Telefone = dto.Telefone
                            };
                            await dbConnection.InsertAsync(contact);

                            Console.WriteLine("Registro inserido com sucesso!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        }
    }
}