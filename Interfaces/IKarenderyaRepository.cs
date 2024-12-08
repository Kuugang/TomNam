using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IKarenderyaRepository
    {
        Task<Karenderya> Create(Karenderya karenderya);
    }
}