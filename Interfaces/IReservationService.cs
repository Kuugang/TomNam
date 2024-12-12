using System.Security.Claims;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces
{
    public interface IReservationService
    {
        Task<Reservation?> GetById(Guid ReservationId);
        Task<Reservation> CreateReservation(ReservationRequestDTO.Create request, ClaimsPrincipal user);
        Task<List<Reservation>> GetAllReservations(ClaimsPrincipal User);
        Task UpdateReservationStatus(Guid ReservationId, string Status);
    }
}