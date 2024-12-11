using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

namespace TomNam.Services{
    public class CartItemService: ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        public CartItemService(ICartItemRepository cartItemRepository){
            _cartItemRepository = cartItemRepository;
        }
        public async Task<CartItem?> GetById(Guid Id, string UserId){
            return await _cartItemRepository.GetById(Id, UserId);
        }
        public async Task<List<CartItem>> GetByIds(List<string> CartItemIds, string UserId){
            return await _cartItemRepository.GetByIds(CartItemIds, UserId);
        }

        public async Task<CartItem?> GetByFoodId(Guid FoodId, string UserId){
            return await _cartItemRepository.GetByFoodId(FoodId, UserId);
        }

        public async Task<CartItem> Create(CustomerProfile CustomerProfile, Food Food, CartRequestItemDTO.Create Request){
            var CartItem = new CartItem
            {
                CustomerProfileId = CustomerProfile!.Id,
                Customer = CustomerProfile,
                FoodId = Request.FoodId,
                Food = Food,
                Quantity = Request.Quantity,
            };
            await _cartItemRepository.Create(CartItem);
            return CartItem;
        }

        public async Task<CartItem> Update(CartItem CartItem, CartRequestItemDTO.Update Request){
            CartItem.Quantity = CartItem.Quantity += Request.Quantity;
            CartItem.IsChecked = Request.IsChecked;
            await _cartItemRepository.Update(CartItem);
            return CartItem;
        }
        public async Task<List<CartItemReponseDTO>> GetAll(string UserId){
            var CartItems = await _cartItemRepository.GetAll(UserId);
            return CartItems.Select(ci => new CartItemReponseDTO(ci)).ToList();
        }

        public async Task Delete(CartItem CartItem){
            await _cartItemRepository.Delete(CartItem);
        }

        public async Task DeleteItems(List<CartItem> CartItems)
        {
            await _cartItemRepository.DeleteItems(CartItems);
        }
    }
}