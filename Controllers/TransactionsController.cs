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

            var transactionId = Guid.NewGuid().ToString();
            var transactionResponseDto = new TransactionResponseDTO
            {
                TransactionId = transactionId,
                ReservationId = reservation.Id,
                ReserveItemId = firstReservedItem.Id,
                ReserveItemQuantity = firstReservedItem.Quantity,
                Total = (decimal)reservation.Total,
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

    [HttpGet("{reservationId}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetTransaction(Guid reservationId)
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
            var reservation = await _context.Reservation
                .Include(r => r.ReservedItems) 
                .ThenInclude(ri => ri.Food)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Message = "Transaction retrieval failed",
                    Error = $"Reservation with ID {reservationId} not found"
                });
            }

            if (reservation.CustomerProfileId != Guid.Parse(userId))
            {
                return Forbid();
            }

            var firstReservedItem = reservation.ReservedItems.FirstOrDefault();
            if (firstReservedItem == null)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Message = "Transaction retrieval failed",
                    Error = "No reserved items found in the reservation"
                });
            }

            var transactionResponseDto = new TransactionResponseDTO
            {
                TransactionId = Guid.NewGuid().ToString(),
                ReservationId = reservation.Id,
                ReserveItemId = firstReservedItem.Id,
                ReserveItemQuantity = firstReservedItem.Quantity,
                Total = (decimal)reservation.Total,
                DateTime = reservation.ReserveDateTime
            };

            return Ok(new SuccessResponseDTO
            {
                Message = "Transaction retrieved successfully",
                Data = transactionResponseDto
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