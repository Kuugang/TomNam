using System.Security.Claims;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Exceptions;

namespace TomNam.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUserService _userService;
        private readonly ICartItemService _cartItemService;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(
            IUserService userService,
            ICartItemService cartItemService,
            IReservationRepository reservationRepository,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _cartItemService = cartItemService;
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Reservation?> GetById(Guid ReservationId)
        {
            var reservation = await _reservationRepository.GetById(ReservationId);
            return reservation;
        }

        public async Task<Reservation> CreateReservation(ReservationRequestDTO.Create request, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromToken(user);
            var customerProfile = await _userService.GetCustomerProfile(userId!);
            var cartItems = await _cartItemService.GetByIds(request.CartItemIds, userId!);

            if (cartItems.Count != request.CartItemIds.Count)
            {
                throw new ApplicationExceptionBase(
                    "One or more cart items not found or do not belong to the customer.",
                    "Reservation creation failed.",
                    StatusCodes.Status400BadRequest
                );
            }

            var karenderya = cartItems.First().Food.Karenderya;

            double total = cartItems.Sum(ci => ci.Food.UnitPrice * ci.Quantity);

            var reservation = new Reservation
            {
                Customer = customerProfile!,
                CustomerProfileId = customerProfile!.Id,
                Karenderya = karenderya,
                KarenderyaId = karenderya.Id,
                ReserveDateTime = request.ReserveDateTime,
                Total = total,
                ModeOfPayment = request.ModeOfPayment,
                Status = "Pending"
            };

            List<ReservedItem> reservedItems = new List<ReservedItem>();

            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _reservationRepository.AddReservationAsync(reservation);
                foreach (var item in cartItems)
                {
                    var reservedItem = new ReservedItem
                    {
                        ReservationId = reservation.Id,
                        FoodId = item.FoodId,
                        Quantity = item.Quantity
                    };
                    await _reservationRepository.AddReservedItemAsync(reservedItem);
                    reservedItems.Add(reservedItem);
                }

                await _cartItemService.DeleteItems(cartItems);
            });

            reservation.ReservedItems = reservedItems;

            return reservation;
        }

        public async Task<List<Reservation>> GetAllReservations(ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);
            var CustomerProfile = await _userService.GetCustomerProfile(UserId!);
            var reservedItems = await _reservationRepository.GetAllReservedItemsAsync(CustomerProfile!.Id);
            var Reservations = await _reservationRepository.GetReservationsAsync(CustomerProfile!.Id);

            foreach (var reservation in Reservations)
            {
                reservation.ReservedItems = reservedItems
                    .Where(ri => ri.ReservationId == reservation.Id)
                    .ToList();
            }
            return Reservations;
        }

        public async Task UpdateReservationStatus(Guid ReservationId, string Status)
        {
            var Reservation = await _reservationRepository.GetById(ReservationId);
            if (Reservation == null)
            {
                throw new ApplicationExceptionBase(
                    "Reservation not found",
                    "Update reservation status failed.",
                    StatusCodes.Status404NotFound
                );
            }
            Reservation.Status = Status;
            await _reservationRepository.UpdateReservation(Reservation);
        }
    }
}