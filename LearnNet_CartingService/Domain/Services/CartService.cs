using AutoMapper;
using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Domain.Services
{
	public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;
        private readonly IValidator<CartItem> _cartItemValidator;
        private readonly IValidator<CartEntity> _cartEntityValidator;

        public CartService(ICartRepository repository,
                           IMapper mapper,
                           ILogger<CartService> logger,
                           IValidator<CartItem> cartItemValidator,
                           IValidator<CartEntity> cartEntityValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _cartItemValidator = cartItemValidator;
            _cartEntityValidator = cartEntityValidator;
        }

        public async Task<bool> AddCartItemAsync(int cartId, CartItemDTO cartItemDTO)
        {
            var entity = _mapper.Map<CartItem>(cartItemDTO);

            var validationResult = _cartItemValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _repository.AddCartItemAsync(cartId, entity);

            return result;
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllCartItemsAsync(int cartId)
        {
            var cartEntity = await _repository.GetCartItemsAsync(cartId);

            if (cartEntity == null)
            {
                return Enumerable.Empty<CartItemDTO>();
            }

            var result = cartEntity.Items.Select(_mapper.Map<CartItemDTO>);

            return result;
        }

        public async Task<bool> RemoveCartItemAsync(int cartId, int cartItemId)
        {
            var result = await _repository.RemoveCartItemAsync(cartId, cartItemId);

            return result;
        }
    }
}
