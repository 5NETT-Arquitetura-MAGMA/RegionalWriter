using Dapper.FastCrud;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;
using System.Data;

namespace RegionalWriter.Queue
{
    public class RegionalDeleteConsumer
    {
        private readonly IConfiguration _configuration;

        public RegionalDeleteConsumer(IConfiguration configuration)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _configuration = configuration;
        }

        public async Task Execute(RegionalDeleteDto dto)
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
                            await dbConnection.DeleteAsync(contact);
                            Console.WriteLine("Registro removido com sucesso!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao remover contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        }
    }
}