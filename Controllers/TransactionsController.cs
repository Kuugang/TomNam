using System;
using System.Security.Claims; // Add this for ClaimTypes and FindFirstValue
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        DataContext context,
        IUserService userService,
        ILogger<TransactionController> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDTO.Create request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound(new ErrorResponseDTO
            {
                Message = "Transaction creation failed",
                Error = "User ID not found"
            });
        }

        try
        {
            var reservation = await _context.Reservation
                .Include(r => r.ReservedItems) 
                .ThenInclude(ri => ri.Food)
                .FirstOrDefaultAsync(r => r.Id == request.ReservationId);

            if (reservation == null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Message = "Transaction creation failed",
                    Error = $"Reservation with ID {request.ReservationId} not found"
                });
            }

            var firstReservedItem = reservation.ReservedItems.FirstOrDefault();
            if (firstReservedItem == null)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Message = "Transaction creation failed",
                    Error = "No reserved items found in the reservation"
                });
            }

            var transaction = new Transaction{
                ReservationId = reservation.Id,
                Reservation = reservation,
                Total = reservation.Total,
                DateTime = reservation.ReserveDateTime
            };

            await _context.Transaction.AddAsync(transaction);
            await _context.SaveChangesAsync();
            var transactionResponseDto = new TransactionResponseDTO
            {
                TransactionId = transaction.Id,
                ReservationId = reservation.Id,
                Total = reservation.Total,
                DateTime = reservation.ReserveDateTime
            };

            return Ok(new SuccessResponseDTO
            {
                Message = "Transaction created successfully",
                Data = transactionResponseDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Transaction creation failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }

    [HttpGet]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetTransaction()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound(new ErrorResponseDTO
            {
                Message = "Transaction retrieval failed",
                Error = "User ID not found"
            });
        }

        try
        {   
           var customerProfile = await _context.CustomerProfile
                .FirstOrDefaultAsync(c => c.UserId == userId);

           var userTransactions = await _context.Transaction
            .Include(t => t.Reservation)
            .ThenInclude(r => r.Customer)
            .Where(t => t.Reservation.CustomerProfileId == customerProfile.Id)
            .ToListAsync();
            
            List<TransactionResponseDTO> responseTransactions = new();
            foreach(var transaction in userTransactions){ 
                responseTransactions.Add(new TransactionResponseDTO {
                    TransactionId = transaction.Id,
                    ReservationId = transaction.Reservation.Id,
                    Total = transaction.Reservation.Total,
                    DateTime = transaction.Reservation.ReserveDateTime
                });
            }
            
            return Ok(new SuccessResponseDTO
            {
                Message = "Transaction retrieved successfully",
                Data = userTransactions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Transaction retrieval failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }
}