using RegionalData.Core.Interface.Service;
using RegionalData.Core.Models.Entity;
using RegionalData.Data;

namespace RegionalData.Services
{
    public class ContactService : IContactService
    {
        private readonly IConfiguration _configuration;

        public ContactService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var repository = new ContactRepository(connectionString);
                return await repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<Contact> GetAsync(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var repository = new ContactRepository(connectionString);
                return await repository.GetAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<List<Contact>> GetByEmail(string email)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var repository = new ContactRepository(connectionString);
                return await repository.GetByEmail(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao buscar cidades. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
                return null;
            }
        }
    }
}