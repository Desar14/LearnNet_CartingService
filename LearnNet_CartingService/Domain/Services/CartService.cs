using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Domain.Services
{
	public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly ILogger<CartService> _logger;
        private readonly IValidator<CartItem> _cartItemValidator;
        private readonly IValidator<CartEntity> _cartEntityValidator;

        public CartService(ICartRepository repository,
                           ILogger<CartService> logger,
                           IValidator<CartItem> cartItemValidator,
                           IValidator<CartEntity> cartEntityValidator)
        {
            _repository = repository;
            _logger = logger;
            _cartItemValidator = cartItemValidator;
            _cartEntityValidator = cartEntityValidator;
        }

        public async Task<bool> AddCartItemAsync(string cartId, CartItemDTO cartItemDTO)
        {
            var entity = CartItemDTO.MapTo(cartItemDTO);

            var validationResult = _cartItemValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _repository.AddCartItemAsync(cartId, entity);

            return result;
        }

        public async Task<IList<CartItemDTO>?> GetAllCartItemsAsync(string cartId)
        {
            var cartEntity = await _repository.GetCartWithItemsAsync(cartId);

            if (cartEntity == null)
            {
                return null;
            }

            var result = cartEntity.Items.Select(CartItemDTO.MapFrom).ToList();

            return result;
        }

        public async Task<CartDTO?> GetCartAsync(string cartId)
        {
            var cartEntity = await _repository.GetCartWithItemsAsync(cartId);

            if (cartEntity == null)
            {
                return null;
            }

            return CartDTO.MapFrom(cartEntity);
        }

        public async Task<bool> RemoveCartItemAsync(string cartId, int cartItemId)
        {
            var result = await _repository.RemoveCartItemAsync(cartId, cartItemId);

            return result;
        }

        public async Task<bool> UpdateItemsAsync(CartItemDTO cartItemDTO)
        {
            var entity = CartItemDTO.MapTo(cartItemDTO);

            var validationResult = _cartItemValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _repository.UpdateCartItemsAsync(entity);

            return result;
        }
    }
}
