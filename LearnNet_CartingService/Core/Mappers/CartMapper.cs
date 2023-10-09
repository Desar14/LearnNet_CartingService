using AutoMapper;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.Mappers
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<CartItem, CartItemDTO>();
            CreateMap<CartItemDTO, CartItem>();
        }
    }
}
