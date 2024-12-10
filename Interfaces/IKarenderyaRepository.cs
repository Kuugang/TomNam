using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IKarenderyaRepository
    {
        Task<Karenderya> Create(Karenderya karenderya);
        Task<Karenderya> GetByIdAsync(Guid id);
        Task<Karenderya> GetByUserId(string userId);
        Task<List<Karenderya>> GetAll(KarenderyaRequestDTO.Read filters);
    }
}