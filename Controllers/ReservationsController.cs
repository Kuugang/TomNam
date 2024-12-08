using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<ReservationController> _logger;

    public ReservationController(
        DataContext context, 
        IUserService userService,
        ILogger<ReservationController> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDTO.Create request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound(new ErrorResponseDTO
            {
                Message = "Reservation creation failed",
                Error = "User ID not found"
            });
        }

        try 
        {
            var customer = await _context.CustomerProfile
                .Where(k => k.UserId == userId)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Message = "Reservation creation failed",
                    Error = $"Customer profile for user {userId} not found"
                });
            }

            var cartItems = await _context.CartItem
                .Where(ci => request.CartItemIds.Contains(ci.Id.ToString()) && ci.CustomerProfileId == customer.Id)
                .Include(ci => ci.Food)
                .ThenInclude(f => f.Karenderya)
                .ToListAsync();

            if (cartItems.Count != request.CartItemIds.Count)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Message = "Reservation creation failed",
                    Error = "One or more cart items not found or do not belong to the customer"
                });
            }

            var karenderya = cartItems.First().Food.Karenderya;
            double total = cartItems.Sum(ci => ci.Food.UnitPrice * ci.Quantity);

            var conflictingReservation = await _context.Reservation
                .AnyAsync(r => 
                    r.KarenderyaId == karenderya.Id && 
                    r.ReserveDateTime == request.ReserveDateTime);

            if (conflictingReservation)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Message = "Reservation creation failed",
                    Error = "Selected time slot is already booked"
                });
            }

            var reservation = new Reservation
            {
                CustomerProfileId = customer.Id,
                Customer = customer,  
                KarenderyaId = karenderya.Id,
                Karenderya = karenderya,  
                ReserveDateTime = request.ReserveDateTime,
                Total = total,
                ModeOfPayment = request.ModeOfPayment
            };

            

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try 
                {
                    await _context.Reservation.AddAsync(reservation);
                    await _context.SaveChangesAsync();

                    foreach (var item in cartItems){
                        var ReservedItem = new ReservedItem{
                            ReservationId = reservation.Id,
                            Reservation = reservation,
                            FoodId = item.FoodId,
                            Food = item.Food,
                            Quantity = item.Quantity     
                        };

                        await _context.ReservedItem.AddAsync(ReservedItem);
                        await _context.SaveChangesAsync();
                    }

                    _context.CartItem.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating reservation");
                    throw;
                }
            }

            return StatusCode(Status201Created, new SuccessResponseDTO
            {
                Message = "Reservation created successfully",
                Data = new ReservationResponseDTO
                {
                    Id = reservation.Id,
                    CustomerProfileId = customer.Id,
                    Customer = customer,  
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
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating reservation");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Reservation creation failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }

    [HttpGet("{Id}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetReservations([FromRoute] Guid Id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return StatusCode(Status404NotFound, new ErrorResponseDTO
            {
                Message = "Get reservations failed",
                Error = "User ID not found"
            });
        }

        try
        {
            var customer = await _context.CustomerProfile
                .Where(k => k.UserId == userId)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return StatusCode(Status404NotFound, new ErrorResponseDTO
                {
                    Message = "Get reservations failed",
                    Error = $"Customer profile for user {userId} not found"
                });
            }

            var ReservedItems = await _context.ReservedItem
                .Include(ri => ri.Reservation)
                .Include(ri => ri.Food)
                .Where(ri => ri.ReservationId == Id)
                .ToListAsync();
                    
            var reservations =  _context.Reservation
            .Where(r => r.CustomerProfileId == customer.Id && r.Id == Id)
            .Select(r => new ReservationResponseDTO
            {
                Id = r.Id,
                CustomerProfileId = r.CustomerProfileId,
                Customer = r.Customer,
                Karenderya = r.Karenderya,
                ReserveDateTime = r.ReserveDateTime,
                Total = r.Total,
                ModeOfPayment = r.ModeOfPayment,
                ReservedItems = ReservedItems
            });
            

            return Ok(new SuccessResponseDTO
            {
                Message = "Reservations retrieved successfully",
                Data = reservations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservations");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Get reservations failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }

    // [HttpPut("{id}/cancel")]
    // [HttpPut("{id}")]
}