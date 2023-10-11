using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.Interfaces
{
	public interface ICartRepository
	{
		public Task<bool> AddCartItemAsync(int cartId, CartItem cartItem);

		public Task<bool> RemoveCartItemAsync(int cartId, int cartItemId);

		public Task<CartEntity> GetCartItemsAsync(int cartId);
	}
}
