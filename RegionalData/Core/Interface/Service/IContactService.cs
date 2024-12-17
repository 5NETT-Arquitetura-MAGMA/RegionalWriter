using RegionalData.Core.Models.Entity;

namespace RegionalData.Core.Interface.Service
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync();

        Task<Contact> GetAsync(int id);

        Task<List<Contact>> GetByEmail(string email);
    }
}