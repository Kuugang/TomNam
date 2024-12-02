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
[Route("api/cart")]
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

            var a = await _context.CartItem.FirstOrDefaultAsync(a => a.FoodId == request.FoodId);
            if(a != null){
                a.Quantity = (int)(a.Quantity += request.Quantity);

                await _context.SaveChangesAsync();

                return Ok(new SuccessResponseDTO
                {
                    Message = "Cart item updated added",
                    Data = new CartItemReponseDTO
                    {
                        Id = a.Id,
                        Customer = a.Customer,
                        Food = a.Food,
                        Quantity = a.Quantity,
                        IsChecked = true
                    }
                });
            }else{
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
        }
        catch (Exception ex)
        {
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
    [HttpGet("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetCartItems()
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserId))
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Get cart items failed",
                    Error = "User ID not found"
                }
            );
        }

        try
        {
            var Customer = await _context.CustomerProfile
                .Where(k => k.UserId == UserId)
                .FirstOrDefaultAsync();

            if (Customer == null)
            {
                return StatusCode(
                    StatusCodes.Status404NotFound,
                    new ErrorResponseDTO
                    {
                        Message = "Get cart items failed",
                        Error = $"Customer profile for user {UserId} not found"
                    }
                );
            }

            var CartItems = await _context.CartItem
                .Where(ci => ci.CustomerProfileId == Customer.Id)
                .Include(ci => ci.Food)
                .Select(ci => new CartItemReponseDTO
                {
                    Id = ci.Id,
                    Customer = Customer,
                    Food = ci.Food,
                    Quantity = ci.Quantity,
                    IsChecked = ci.IsChecked
                })
                .ToListAsync();

            return Ok(new SuccessResponseDTO
            {
                Message = "Cart items retrieved successfully",
                Data = CartItems
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving cart items: {ex.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Get cart items failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }

    // [HttpPut]
    [HttpPut("{cartItemId}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] CartRequestItemDTO.Update request)
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserId))
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Update cart item failed",
                    Error = "User ID not found"
                }
            );
        }

        try
        {
            var Customer = await _context.CustomerProfile
                .Where(k => k.UserId == UserId)
                .FirstOrDefaultAsync();

            if (Customer == null)
            {
                return StatusCode(
                    StatusCodes.Status404NotFound,
                    new ErrorResponseDTO
                    {
                        Message = "Update cart item failed",
                        Error = $"Customer profile for user {UserId} not found"
                    }
                );
            }

            var CartItem = await _context.CartItem
                .Where(ci => ci.Id == cartItemId && ci.CustomerProfileId == Customer.Id)
                .Include(ci => ci.Food)
                .FirstOrDefaultAsync();

            if (CartItem == null)
            {
                return StatusCode(
                    StatusCodes.Status404NotFound,
                    new ErrorResponseDTO
                    {
                        Message = "Update cart item failed",
                        Error = $"Cart item with ID {cartItemId} not found"
                    }
                );
            }

            // Update only the properties that are provided
            if (request.Quantity > 0)
            {
                CartItem.Quantity = request.Quantity;
            }

            CartItem.IsChecked = request.IsChecked;

            await _context.SaveChangesAsync();

            return Ok(new SuccessResponseDTO
            {
                Message = "Cart item updated successfully",
                Data = new CartItemReponseDTO
                {
                    Id = CartItem.Id,
                    Customer = Customer,
                    Food = CartItem.Food,
                    Quantity = CartItem.Quantity,
                    IsChecked = CartItem.IsChecked
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating cart item: {ex.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Update cart item failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }

    // [HttpDelete]
    [HttpDelete("{id}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserId))
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Delete cart item failed",
                    Error = "User ID not found"
                }
            );
        }

        try
        {
            var Customer = await _context.CustomerProfile
                .Where(k => k.UserId == UserId)
                .FirstOrDefaultAsync();

            if (Customer == null)
            {
                return StatusCode(
                    StatusCodes.Status404NotFound,
                    new ErrorResponseDTO
                    {
                        Message = "Delete cart item failed",
                        Error = $"Customer profile for user {UserId} not found"
                    }
                );
            }

            var CartItem = await _context.CartItem
                .Where(ci => ci.Id == id && ci.CustomerProfileId == Customer.Id)
                .FirstOrDefaultAsync();

            if (CartItem == null)
            {
                return StatusCode(
                    StatusCodes.Status404NotFound,
                    new ErrorResponseDTO
                    {
                        Message = "Delete cart item failed",
                        Error = $"Cart item with ID {id} not found"
                    }
                );
            }

            _context.CartItem.Remove(CartItem);
            await _context.SaveChangesAsync();

            return Ok(new SuccessResponseDTO
            {
                Message = "Cart item deleted successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting cart item: {ex.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Delete cart item failed",
                    Error = "An unexpected error occurred"
                }
            );
        }
    }
}
