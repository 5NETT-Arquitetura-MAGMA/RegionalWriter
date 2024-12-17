using RegionalData.Core.Models.Entity;

namespace RegionalData.Core.Interface.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();

        Task<Contact> GetAsync(int id);

        Task<List<Contact>> GetByEmail(string email);
    }
}