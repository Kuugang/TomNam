using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/cart")]
public class CartItemsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly IFoodService _foodService;
    private readonly ICartItemService _cartItemService;

    public CartItemsController(DataContext context, IUserService userService, IFoodService foodService, ICartItemService cartItemService)
    {
        _context = context;
        _userService = userService;
        _foodService = foodService;
        _cartItemService = cartItemService;
    }

    [HttpPost("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> Create([FromBody] CartRequestItemDTO.Create request)
    {
        var UserId = _userService.GetUserIdFromToken(User);

        var Food = await _foodService.GetById(request.FoodId);

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

        var Customer = await _userService.GetCustomerProfile(UserId!);
        var CartItem = await _cartItemService.GetByFoodId(request.FoodId, UserId!);

        if (CartItem != null)
        {
            await _cartItemService.Update(CartItem, new CartRequestItemDTO.Update
            {
                Quantity = CartItem.Quantity += request.Quantity
            });

            return Ok(new SuccessResponseDTO
            {
                Message = "Cart item updated added",
                Data = new CartItemReponseDTO(CartItem)
            });
        }
        else
        {
            CartItem = await _cartItemService.Create(Customer!, Food, request);

            return Ok(new SuccessResponseDTO
            {
                Message = "Cart item successfully added",
                Data = new CartItemReponseDTO(CartItem)
            });
        }
    }

    // [HttpGet]
    [HttpGet("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetCartItems()
    {
        var UserId = _userService.GetUserIdFromToken(User);

        var CartItemsResponse = await _cartItemService.GetAll(UserId!);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart items retrieved successfully",
            Data = CartItemsResponse
        });
    }

    // [HttpPut]
    [HttpPut("{cartItemId}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] CartRequestItemDTO.Update request)
    {
        var UserId = _userService.GetUserIdFromToken(User);
        var CartItem = await _cartItemService.GetById(cartItemId, UserId!);

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

        CartItem = await _cartItemService.Update(CartItem, request);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart item updated successfully",
            Data = new CartItemReponseDTO(CartItem)
        });

    }

    // [HttpDelete]
    [HttpDelete("{id}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        var UserId = _userService.GetUserIdFromToken(User);
        var CartItem = await _cartItemService.GetById(id, UserId!);

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
        await _cartItemService.Delete(CartItem);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart item deleted successfully",
        });
    }
}
