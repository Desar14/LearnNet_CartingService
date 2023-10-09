using AutoMapper;
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

        public CartService(ICartRepository repository, IMapper mapper, ILogger<CartService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddCartItemAsync(int cartId, CartItemDTO cartItemDTO)
        {
            var entity = _mapper.Map<CartItem>(cartItemDTO);

            var result = await _repository.AddCartItemAsync(cartId, entity);

            return result;
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllCartItemsAsync(int cartId)
        {
            var result = (await _repository.GetCartItemsAsync(cartId)).Items.Select(_mapper.Map<CartItemDTO>);
            return result;
        }

        public async Task<bool> RemoveCartItemAsync(int cartId, int cartItemId)
        {
            var result = await _repository.RemoveCartItemAsync(cartId, cartItemId);

            return result;
        }
    }
}
