using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.Interfaces
{
	public interface ICartRepository
	{
		public Task<bool> AddCartItemAsync(string cartId, CartItem cartItem);

		public Task<bool> RemoveCartItemAsync(string cartId, int cartItemId);

		public Task<CartEntity?> GetCartWithItemsAsync(string cartId);
    }
}
