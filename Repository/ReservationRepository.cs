using Microsoft.EntityFrameworkCore;

using TomNam.Models;
using TomNam.Interfaces;
using TomNam.Data;

namespace TomNam.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DataContext _context;

        public ReservationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetById(Guid ReservationId){
            var reservation = await _context.Reservation
                .Include(r => r.Karenderya)
                .Include(r => r.ReservedItems)
                .ThenInclude(ri => ri.Food)
                .FirstOrDefaultAsync(r => r.Id == ReservationId);
            return reservation;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task AddReservedItemAsync(ReservedItem reservedItem)
        {
            await _context.ReservedItem.AddAsync(reservedItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReservedItem>> GetAllReservedItemsAsync(Guid customerProfileId)
        {
            return await _context.ReservedItem
                .Include(ri => ri.Reservation)
                .Include(ri => ri.Food)
                .Where(ri => ri.Reservation.CustomerProfileId == customerProfileId)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsAsync(Guid customerProfileId)
        {
            return await _context.Reservation
                .Where(r => r.CustomerProfileId == customerProfileId)
                .ToListAsync();
        }

        public Task UpdateReservation(Reservation reservation)
        {
            _context.Reservation.Update(reservation);
            return _context.SaveChangesAsync();
        }
    }
}