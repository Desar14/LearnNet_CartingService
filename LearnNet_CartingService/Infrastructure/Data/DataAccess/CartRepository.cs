using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Domain.Entities;
using LiteDB;

namespace LearnNet_CartingService.Infrastructure.Data.DataAccess
{
	public class CartRepository : ICartRepository
	{
		protected readonly ILogger<CartRepository> _logger;
		protected readonly IConfiguration _configuration;
		protected readonly LiteDatabase _liteDb;

		public CartRepository(ILogger<CartRepository> logger, IConfiguration configuration, ILiteDbContext liteDbContext)
		{
			_logger = logger;
			_configuration = configuration;
			_liteDb = liteDbContext.Database;
		}

		public Task<bool> AddCartItemAsync(int cartId, CartItem cartItem)
		{
			var col = _liteDb.GetCollection<CartEntity>("Carts");
			var existingCart = col.FindById(cartId);
			
			if (existingCart == null)
			{
				existingCart = new CartEntity
				{
					Id = cartId
				};
				col.Insert(existingCart);
			}

			existingCart.Items.Add(cartItem);

			var result = col.Update(existingCart);
			
			return Task.FromResult(result);
		}

		public Task<CartEntity> GetCartItemsAsync(int cartId)
		{
			var col = _liteDb.GetCollection<CartEntity>("Carts");
			var existingCart = col.FindById(cartId);

			return Task.FromResult(existingCart);
		}

		public Task<bool> RemoveCartItemAsync(int cartId, int cartItemId)
		{
			var col = _liteDb.GetCollection<CartEntity>("Carts");
			var existingCart = col.FindById(cartId);
			
			if (existingCart == null)
			{
				return Task.FromResult(false);
			}

			var existingCartItem = existingCart.Items.FirstOrDefault(x => x.Id == cartItemId);
			if (existingCartItem == null)
			{
				return Task.FromResult(false);
			}

			existingCart.Items.Remove(existingCartItem);

			var result = col.Update(existingCart);
			
			return Task.FromResult(result);
		}
	}
}
