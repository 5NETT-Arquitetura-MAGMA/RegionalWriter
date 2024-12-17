using CityData.Core.Interface.Service;
using CityData.Core.Models.Entity;
using CityData.Data;

namespace CityData.Services
{
    public class CityService : ICityService
    {
        private readonly IConfiguration _configuration;

        public CityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Cidade>> GetAllAsync()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var repository = new CityRepository(connectionString);
                return await repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<Cidade> GetAsync(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var repository = new CityRepository(connectionString);
                return await repository.GetAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidade. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return null;
            }
        }
    }
}