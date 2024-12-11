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
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IUserService userService,
            ICartItemService cartItemService,
            IReservationRepository reservationRepository,
            IUnitOfWork unitOfWork,
            ILogger<ReservationService> logger)
        {
            _userService = userService;
            _cartItemService = cartItemService;
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SuccessResponseDTO> CreateReservationAsync(ReservationRequestDTO.Create request, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromToken(user);
            var customerProfile = await _userService.GetCustomerProfile(userId!);
            var cartItems = await _cartItemService.GetByIds(request.CartItemIds, userId!);

            if (cartItems.Count != request.CartItemIds.Count)
            {
                throw new ApplicationExceptionBase(
                    "One or more cart items not found or do not belong to the customer.",
                    StatusCodes.Status400BadRequest
                );
            }

            var karenderya = cartItems.First().Food.Karenderya;

            if (await _reservationRepository.HasConflictingReservationAsync(karenderya.Id, request.ReserveDateTime))
            {
                throw new ApplicationExceptionBase(
                    "Selected time slot is already booked.",
                    StatusCodes.Status409Conflict
                );
            }

            double total = cartItems.Sum(ci => ci.Food.UnitPrice * ci.Quantity);
            var reservation = new Reservation
            {
                Customer = customerProfile!,
                CustomerProfileId = customerProfile!.Id,
                Karenderya = karenderya,
                KarenderyaId = karenderya.Id,
                ReserveDateTime = request.ReserveDateTime,
                Total = total,
                ModeOfPayment = request.ModeOfPayment
            };

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
                }

                await _cartItemService.DeleteItems(cartItems);
            });

            return new SuccessResponseDTO
            {
                Message = "Reservation created successfully",
                Data = new ReservationResponseDTO
                {
                    Id = reservation.Id,
                    Customer = customerProfile,
                    CustomerProfileId = customerProfile.Id,
                    Karenderya = karenderya,
                    ReserveDateTime = request.ReserveDateTime,
                    Total = total,
                    ModeOfPayment = request.ModeOfPayment,
                    ReservedItems = cartItems.Select(ci => new ReservedItem
                    {
                        Id = ci.Id,
                        FoodId = ci.FoodId,
                        Food = ci.Food,
                        Quantity = ci.Quantity
                    }).ToList()
                }
            };
        }

        public async Task<SuccessResponseDTO> GetAllReservationsAsync(ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);
            var CustomerProfile = await _userService.GetCustomerProfile(UserId!);
            var reservedItems = await _reservationRepository.GetAllReservedItemsAsync(CustomerProfile!.Id);
            var ReservationsResponse = (await _reservationRepository.GetReservationsAsync(CustomerProfile!.Id))
            .Select(r => new ReservationResponseDTO
            {
                Id = r.Id,
                CustomerProfileId = r.CustomerProfileId,
                Customer = r.Customer,
                Karenderya = r.Karenderya,
                ReserveDateTime = r.ReserveDateTime,
                Total = r.Total,
                ModeOfPayment = r.ModeOfPayment
            })
            .ToList();

            foreach (var reservation in ReservationsResponse)
            {
                reservation.ReservedItems = reservedItems
                    .Where(ri => ri.ReservationId == reservation.Id)
                    .ToList();
            }

            return new SuccessResponseDTO {
                Message = "Reservations retrieved successfully",
                Data = ReservationsResponse
            };
        }
    }
}