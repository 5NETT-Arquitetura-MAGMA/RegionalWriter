using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RegionalWriter.Model.Entity;
using RegionalWriter.Model.View;

namespace RegionalWriter.Test;

public class ConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration _configuration;

    public ConnectionStringProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name);
    }

    public void Insert(RegionalDto dto)
    {
        var connectionString = GetConnectionString("DefaultConnection");
        using (var connection = new SqlConnection(connectionString))
        {
            var query = "INSERT INTO Regional (Nome, Telefone, DDD, Email, Estado, Cidade) VALUES (@Nome, @Telefone, @DDD, @Email, @Estado, @Cidade)";
            connection.Execute(query, new { dto.Nome, dto.Telefone, dto.DDD, dto.Email, dto.Estado, dto.Cidade });
        }
    }

    public async Task<Contact> GetContactAsync(int id)
    {
        var connectionString = GetConnectionString("DefaultConnection");
        using (var connection = new SqlConnection(connectionString))
        {
            var query = "SELECT * FROM Contacts WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Contact>(query, new { Id = id });
        }
    }

    public async Task UpdateContactAsync(Contact contact)
    {
        var connectionString = GetConnectionString("DefaultConnection");
        using (var connection = new SqlConnection(connectionString))
        {
            var query = "UPDATE Contacts SET Nome = @Nome, Telefone = @Telefone, DDD = @DDD, Email = @Email, Estado = @Estado, Cidade = @Cidade WHERE Id = @Id";
            await connection.ExecuteAsync(query, contact);
        }
    }

    public async Task DeleteContactAsync(Contact contact)
    {
        var connectionString = GetConnectionString("DefaultConnection");
        using (var connection = new SqlConnection(connectionString))
        {
            var query = "DELETE FROM Contacts WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { Id = contact.Id });
        }
    }
}