using System.Security.Claims;
using TomNam.Exceptions;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

namespace TomNam.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUserService _userService;
        private readonly IFoodService _foodService;
        private readonly ICartItemRepository _cartItemRepository;
        public CartItemService(IUserService userService, IFoodService foodService, ICartItemRepository cartItemRepository)
        {
            _userService = userService;
            _foodService = foodService;
            _cartItemRepository = cartItemRepository;
        }
        public async Task<CartItem?> GetById(Guid Id, string UserId)
        {
            return await _cartItemRepository.GetById(Id, UserId);
        }
        public async Task<List<CartItem>> GetByIds(List<Guid> CartItemIds, string UserId)
        {
            return await _cartItemRepository.GetByIds(CartItemIds, UserId);
        }

        public async Task<CartItem?> GetByFoodId(Guid FoodId, string UserId)
        {
            return await _cartItemRepository.GetByFoodId(FoodId, UserId);
        }

        public async Task<CartItem> Create(CartRequestItemDTO.Create Request, ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);

            var Food = await _foodService.GetById(Request.FoodId);
            if (Food == null)
            {
                throw new ApplicationExceptionBase(
                    $"Food with ID {Request.FoodId} not found",
                    "Cart item create failed",
                    StatusCodes.Status404NotFound
                );
            }

            var CartItem = await GetByFoodId(Request.FoodId, UserId!);

            var CustomerProfile = await _userService.GetCustomerProfile(UserId!);

            if (CustomerProfile == null)
            {
                throw new ApplicationExceptionBase(
                    $"Customer profile for User ID {UserId} not found",
                    "Cart item create failed",
                    StatusCodes.Status404NotFound
                );
            }
            if (CartItem != null)
            {
                await Update(CartItem.Id, new CartRequestItemDTO.Update
                {
                    Quantity = CartItem.Quantity + Request.Quantity
                }, User);
                return CartItem;
            }
            else
            {
                CartItem = new CartItem
                {
                    CustomerProfileId = CustomerProfile.Id,
                    Customer = CustomerProfile,
                    FoodId = Request.FoodId,
                    Food = Food,
                    Quantity = Request.Quantity,
                };
                await _cartItemRepository.Create(CartItem);
                return CartItem;
            }
        }

        public async Task<CartItem> Update(Guid CartItemId, CartRequestItemDTO.Update Request, ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);
            var CartItem = await GetById(CartItemId, UserId!);

            if (CartItem == null)
            {
                throw new ApplicationExceptionBase(
                    $"Cart item with ID {CartItemId} not found",
                    "Cart item update failed",
                    StatusCodes.Status404NotFound
                );
            }
            if(Request.Quantity > 0){
                CartItem.Quantity = (int)Request.Quantity;
            }
            if(Request.IsChecked != null){
                CartItem.IsChecked = (bool)Request.IsChecked;
            }
            await _cartItemRepository.Update(CartItem);
            return CartItem;
        }
        public async Task<List<CartItem>> GetAll(ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);
            return await _cartItemRepository.GetAll(UserId!);
        }
        public async Task Delete(Guid CartItemId, ClaimsPrincipal User)
        {
            var UserId = _userService.GetUserIdFromToken(User);
            var CartItem = await GetById(CartItemId, UserId!);

            if (CartItem == null)
            {
                throw new ApplicationExceptionBase(
                    $"Cart item with ID {CartItemId} not found",
                    "Cart item delete failed",
                    StatusCodes.Status404NotFound
                );
            }

            await _cartItemRepository.Delete(CartItem);
        }

        public async Task DeleteItems(List<CartItem> CartItems)
        {
            await _cartItemRepository.DeleteItems(CartItems);
        }
    }
}