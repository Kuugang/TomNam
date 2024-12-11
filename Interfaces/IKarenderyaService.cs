using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IKarenderyaService
    {
        Task<Karenderya> Create(KarenderyaRequestDTO.Create request, User user);
        Task <Karenderya?> GetById(Guid id);
        Task <Karenderya?> GetByOwnerId(string id);
        Task <Karenderya> Update(Karenderya karenderya, KarenderyaRequestDTO.Update request);
        Task<List<KarenderyaResponseDTO>> FilterKarenderya(KarenderyaRequestDTO.Read filters);
        Task<ProofOfBusiness> CreateProofOfBusiness(Karenderya karenderya, KarenderyaRequestDTO.ProofOfBusinessCreateDTO request);
        Task<ProofOfBusiness?> GetProofOfBusiness(Guid karenderyaId);
        Task<ProofOfBusiness> UpdateProofOfBusiness(ProofOfBusiness proofOfBusiness, KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request); 
    }
}