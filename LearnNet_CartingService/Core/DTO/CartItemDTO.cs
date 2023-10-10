using FluentValidation;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.DTO
{
	public class CartItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ItemImageDTO? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public static CartItemDTO MapFrom(CartItem cartItem)
        {
            var dto = new CartItemDTO
            {
                Id = cartItem.Id,
                Name = cartItem.Name,
                Image = new ItemImageDTO
                {
                    Url = cartItem.ImageUrl,
                    AltText = cartItem.ImageText
                },
                Price = cartItem.Price,
                Quantity = cartItem.Quantity
            };

            return dto;
        }

        public static CartItem MapTo(CartItemDTO dto) 
        {
            var entity = new CartItem
            {
                Id = dto.Id,
                Name = dto.Name,
                ImageText = dto.Image?.AltText,
                ImageUrl = dto.Image?.Url,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

            return entity;
        }
    }

    
}
