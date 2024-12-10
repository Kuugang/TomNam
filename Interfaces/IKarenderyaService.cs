using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IKarenderyaService
    {
        Task<IActionResult> Create(KarenderyaRequestDTO.Create request, User user);
       
    }
}