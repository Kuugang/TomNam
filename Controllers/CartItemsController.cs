using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Interfaces;
using TomNam.Models.DTO;

[ApiController]
[Route("api/cart")]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _cartItemService;

    public CartItemsController(ICartItemService cartItemService)
    {
        _cartItemService = cartItemService;
    }

    [HttpPost("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> Create([FromBody] CartRequestItemDTO.Create request)
    {
        var CartItem = await _cartItemService.Create(request, User);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart item successfully added",
            Data = CartItem
        });
    }

    // [HttpGet]
    [HttpGet("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetCartItems()
    {
        var CartItems = await _cartItemService.GetAll(User);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart items retrieved successfully",
            Data = CartItems
        });
    }

    // [HttpPut]
    [HttpPut("{cartItemId}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> UpdateCartItem(Guid CartItemId, [FromBody] CartRequestItemDTO.Update request)
    {
        var CartItem = await _cartItemService.Update(CartItemId, request, User);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart item updated successfully",
            Data = CartItem
        });

    }

    // [HttpDelete]
    [HttpDelete("{CartItemId}")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> DeleteCartItem(Guid CartItemId)
    {
        await _cartItemService.Delete(CartItemId, User);

        return Ok(new SuccessResponseDTO
        {
            Message = "Cart item deleted successfully",
        });
    }
}
