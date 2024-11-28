using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Middlewares;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/cartItems")]
public class CartItemsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserService _userService;

    public CartItemsController(DataContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost("")]
[Authorize(Policy = "CustomerPolicy")]
public async Task<IActionResult> Create([FromBody] CartRequestItemDTO.Create request)
{
    
    // Validate request
    if (request == null || request.FoodId == Guid.Empty || request.Quantity <= 0)
    {
        return BadRequest(new ErrorResponseDTO
        {
            Message = "Cart item create failed",
            Error = "Invalid request",
        });
    }

    var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(UserId))
    {
        return StatusCode(
            StatusCodes.Status404NotFound,
            new ErrorResponseDTO
            {
                Message = "Cart item create failed",
                Error = "User ID not found"
            }
        );
    }

    try 
    {
        var Food = await _context.Food
            .Where(k => k.Id == request.FoodId)
            .FirstOrDefaultAsync();

        if (Food == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Cart item create failed",
                    Error = $"Food with ID {request.FoodId} not found"
                }
            );
        }

        var Customer = await _context.CustomerProfile
            .Where(k => k.UserId == UserId)
            .FirstOrDefaultAsync();

        if (Customer == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Cart item create failed",
                    Error = $"Customer profile for user {UserId} not found"
                }
            );
        }

        var CartItem = new CartItem
        {
            CustomerProfileId = Customer.Id,
            Customer = Customer,
            FoodId = request.FoodId,
            Food = Food,
            Quantity = request.Quantity,
        };

        await _context.CartItem.AddAsync(CartItem);
        await _context.SaveChangesAsync();

         return Ok(new SuccessResponseDTO
        {
            Message = "Cart item successfully added",
            Data = new CartItemReponseDTO
            {
                Id = CartItem.Id,
                Customer = Customer,
                Food = Food,
                Quantity = CartItem.Quantity,
                IsChecked = true
            }
        });
    }
    catch (Exception ex)
    {
        // Log the exception if you have logging set up
        Console.WriteLine($"Error creating cart item: {ex.Message}");
        return StatusCode(
            StatusCodes.Status500InternalServerError,
            new ErrorResponseDTO
            {
                Message = "Cart item create failed",
                Error = "An unexpected error occurred"
            }
        );
    }
}

    // [HttpGet]

    // [HttpPut]

    // [HttpDelete]
}
