using Dapper.FastCrud;
using Microsoft.Data.SqlClient;
using RegionalData.Core.Interface.Repository;
using RegionalData.Core.Models.Entity;
using System.Data;

namespace RegionalData.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository(string connectionString)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _connectionString = connectionString;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                var cidades = await dbConnection.FindAsync<Contact>();
                return cidades.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar contatos. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Contact> GetAsync(int id)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                var cidade = await dbConnection.GetAsync(new Contact { Id = id });
                return cidade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<Contact>> GetByEmail(string email)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                var cidade = await dbConnection.FindAsync<Contact>();
                return cidade?.Where(x => x.Email == email).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                throw;
            }
        }
    }
}