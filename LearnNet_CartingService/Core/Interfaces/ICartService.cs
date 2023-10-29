using LearnNet_CartingService.Core.DTO;

namespace LearnNet_CartingService.Core.Interfaces
{
	public interface ICartService
	{
		Task<IList<CartItemDTO>?> GetAllCartItemsAsync(string cartId);

        Task<CartDTO?> GetCartAsync(string cartId);

        Task<bool> AddCartItemAsync(string cartId, CartItemDTO cartItemDTO);

		Task<bool> RemoveCartItemAsync(string cartId, int cartItemId);
	}
}
