using CityData.Core.Models.Entity;

namespace CityData.Core.Interface.Service
{
    public interface ICityService
    {
        Task<List<Cidade>> GetAllAsync();

        Task<Cidade> GetAsync(int id);
    }
}