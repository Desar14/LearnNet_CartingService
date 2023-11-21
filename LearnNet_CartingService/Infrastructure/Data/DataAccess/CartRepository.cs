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

        public async Task<bool> UpdateCartItemsAsync(CartItem cartItem)
        {
            var col = _liteDb.GetCollection<CartEntity>("Carts");
            var existingCarts = col.Find(x => x.Items.Where(item => item.Id == cartItem.Id).Count() > 0);

            if (existingCarts == null || existingCarts.Count() == 0)
            {
                return true;
            }
            var transactionStarted = _liteDb.BeginTrans();

            if (!transactionStarted)
            {
                throw new Exception("failed to create a transaction");
            }

            foreach (var cart in existingCarts) { 
                foreach (var item in cart.Items.Where(item => item.Id == cartItem.Id))
                {
                    item.Price = cartItem.Price;
                    item.Name = cartItem.Name;
                    item.ImageUrl = cartItem.ImageUrl;
                    item.ImageText = cartItem.ImageText;
                }

                col.Update(cart);
            }

            var result = _liteDb.Commit();

            return result;
        }
    }
}
