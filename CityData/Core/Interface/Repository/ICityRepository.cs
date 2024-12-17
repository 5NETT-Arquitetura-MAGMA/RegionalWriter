using CityData.Core.Models.Entity;

namespace CityData.Core.Interface.Repository
{
    public interface ICityRepository
    {
        Task<List<Cidade>> GetAllAsync();

        Task<Cidade> GetAsync(int id);
    }
}