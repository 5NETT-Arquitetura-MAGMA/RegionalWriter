using CityData.Core.Interface.Repository;
using CityData.Core.Models.Entity;
using Dapper.FastCrud;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CityData.Data
{
    public class CityRepository : ICityRepository
    {
        private readonly string _connectionString;

        public CityRepository(string connectionString)
        {
            OrmConfiguration.DefaultDialect = SqlDialect.MsSql;
            _connectionString = connectionString;
        }

        public async Task<List<Cidade>> GetAllAsync()
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                var cidades = await dbConnection.FindAsync<Cidade>();
                return cidades.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Cidade> GetAsync(int id)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                var cidade = await dbConnection.GetAsync(new Cidade { Id = id });
                return cidade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidade. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                throw;
            }
        }
    }
}