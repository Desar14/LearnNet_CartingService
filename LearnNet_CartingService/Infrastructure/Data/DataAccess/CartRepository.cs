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

        public async Task<bool> AddCartItemAsync(string cartId, CartItem cartItem)
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

            var existingCartItem = existingCart.Items.FirstOrDefault(x => x.Id == cartItem.Id);
            if (existingCartItem != null)
            {
                existingCart.Items.Remove(existingCartItem);
            }

            existingCart.Items.Add(cartItem);

            var result = col.Update(existingCart);

            return result;
        }

        public async Task<CartEntity?> GetCartWithItemsAsync(string cartId)
        {
            var col = _liteDb.GetCollection<CartEntity>("Carts");
            var existingCart = col.FindById(cartId);

            return existingCart;
        }

        public async Task<bool> RemoveCartItemAsync(string cartId, int cartItemId)
        {
            var col = _liteDb.GetCollection<CartEntity>("Carts");
            var existingCart = col.FindById(cartId);

            if (existingCart == null)
            {
                return false;
            }

            var existingCartItem = existingCart.Items.FirstOrDefault(x => x.Id == cartItemId);
            if (existingCartItem == null)
            {
                return false;
            }

            existingCart.Items.Remove(existingCartItem);

            var result = col.Update(existingCart);

            if (existingCart.Items.Count == 0)
            {
                result = col.DeleteMany(x => x.Id == existingCart.Id) > 0;
            }

            return result;
        }
    }
}
