using System.Security.Claims;
using TomNam.Models.DTO;
namespace TomNam.Interfaces
{
    public interface IReservationService
    {
        Task<SuccessResponseDTO> CreateReservationAsync(ReservationRequestDTO.Create request, ClaimsPrincipal user);
        Task<SuccessResponseDTO> GetAllReservationsAsync(ClaimsPrincipal User);
    }
}