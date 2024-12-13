using System.Security.Claims;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IKarenderyaService
    {
        Task<Karenderya> Create(ClaimsPrincipal User, KarenderyaRequestDTO.Create request);
        Task <Karenderya?> GetById(Guid id);
        Task <Karenderya?> GetByOwnerId(string id);
        Task <Karenderya> Update(Guid KarenderyaId, KarenderyaRequestDTO.Update request, ClaimsPrincipal User);
        Task<List<Karenderya>> FilterKarenderyas(KarenderyaRequestDTO.Read filters);
        Task<ProofOfBusiness> CreateProofOfBusiness(Guid KarenderyaId, KarenderyaRequestDTO.ProofOfBusinessCreateDTO request, ClaimsPrincipal User);
        Task<ProofOfBusiness> GetProofOfBusiness(Guid KarenderyaId, ClaimsPrincipal User);
        Task<ProofOfBusiness> UpdateProofOfBusiness(Guid KarenderyaId, KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request, ClaimsPrincipal User); 
        Task<Karenderya> VerifyKarenderya(Guid KarenderyaId); 
    }
}