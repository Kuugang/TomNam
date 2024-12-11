using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IKarenderyaRepository
    {
        Task<Karenderya> Create(Karenderya karenderya);
        Task<Karenderya?> GetByIdAsync(Guid id);
        Task<Karenderya?> GetByOwnerId(string id);
        Task Update(Karenderya karenderya);
        Task<List<Karenderya>> FilterKarenderya(KarenderyaRequestDTO.Read filters);
        Task CreateProofOfBusiness(ProofOfBusiness proofOfBusiness);
        Task <ProofOfBusiness?> GetProofOfBusiness(Guid karenderyaId);
        Task UpdateProofOfBusiness(ProofOfBusiness proofOfBusiness);
    }
}