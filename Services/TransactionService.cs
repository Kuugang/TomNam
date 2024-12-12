using System.Security.Claims;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Exceptions;

namespace TomNam.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUserService _userService;
        private readonly IReservationService _reservationService;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(
            IUserService userService,
            IReservationService reservationService,
            ITransactionRepository transactionRepository)
        {
            _userService = userService;
            _reservationService = reservationService;
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateTransaction(TransactionRequestDTO.Create request, ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);

            var reservation = await _reservationService.GetById(request.ReservationId);
            if (reservation == null)
            {
                throw new ApplicationExceptionBase(
                    "Reservation not found",
                    "Transaction creation failed.",
                    StatusCodes.Status404NotFound
                );
            }

            if (reservation.Status == "Paid")
            {
                throw new ApplicationExceptionBase(
                    "Reservation is already paid",
                    "Transaction creation failed.",
                    StatusCodes.Status400BadRequest
                );
            }

            if (reservation.Status == "Cancelled")
            {
                throw new ApplicationExceptionBase(
                    "Reservation is already cancelled",
                    "Transaction creation failed.",
                    StatusCodes.Status400BadRequest
                );
            }

            var OwnerProfile = await _userService.GetOwnerProfile(UserId!);
            if (reservation.Karenderya.Id != OwnerProfile!.KarenderyaId)
            {
                throw new ApplicationExceptionBase(
                    "You are not authorized to create transaction for this reservation. You are not the owner of the karenderya.",
                    "Transaction creation failed.",
                    StatusCodes.Status401Unauthorized
                );
            }

            var transaction = new Transaction
            {
                ReservationId = reservation.Id,
                Reservation = reservation,
                Total = reservation.Total,
                DateTime = reservation.ReserveDateTime
            };
            await _transactionRepository.CreateTransaction(transaction);
            await _reservationService.UpdateReservationStatus(reservation.Id, "Paid");
            return transaction;
        }

        public async Task<List<Transaction>> GetAllCustomerTransactions(ClaimsPrincipal User)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CustomerProfile = await _userService.GetCustomerProfile(userId!);
            var userTransactions = await _transactionRepository.GetAllCustomerTransactions(CustomerProfile!);
            return userTransactions;
        }
    }
}