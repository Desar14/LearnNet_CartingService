using LearnNet_CartingService.Core.DTO;

namespace LearnNet_CartingService.Core.Interfaces
{
	public interface ICartService
	{
		Task<IEnumerable<CartItemDTO>> GetAllCartItemsAsync(int cartId);

		Task<bool> AddCartItemAsync(int cartId, CartItemDTO cartItemDTO);

		Task<bool> RemoveCartItemAsync(int cartId, int cartItemId);
	}
}
