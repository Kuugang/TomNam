using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces{
    public interface IKarenderyaService
    {
        Task<IActionResult> Create(KarenderyaRequestDTO.Create request);
        // Task<User> Create(RegisterRequest RegisterRequest);
        // Task<string> Login(LoginRequest LoginRequest);
        // Task<string> GenerateToken(User user, string role);
        // Task<string> GetRole(User user);
    }
}