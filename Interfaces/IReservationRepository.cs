using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetById(Guid ReservationId);
        Task AddReservationAsync(Reservation reservation);
        Task AddReservedItemAsync(ReservedItem reservedItem);
        Task<List<ReservedItem>> GetAllReservedItemsAsync(Guid customerProfileId);
        Task<List<Reservation>> GetReservationsAsync(Guid customerProfileId);
        Task UpdateReservation(Reservation reservation);
    }
}
