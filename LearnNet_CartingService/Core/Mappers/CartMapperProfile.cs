using AutoMapper;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.Mappers
{
    public class CartMapperProfile : Profile
    {
        public CartMapperProfile()
        {
            CreateMap<CartItem, CartItemDTO>();
            CreateMap<CartItemDTO, CartItem>();
        }
    }
}
