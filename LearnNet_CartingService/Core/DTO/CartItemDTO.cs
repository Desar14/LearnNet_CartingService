using LearnNet_CartingService.Domain.Entities;
using LearnNet_CartingService.gRPC;
using System.Text.Json.Serialization;

namespace LearnNet_CartingService.Core.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ItemImageDTO? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public bool Updating {  get; set; } = false;

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

        public static CartItemDTO MapFrom(CartItemMessage cartItem)
        {
            var dto = new CartItemDTO
            {
                Id = cartItem.Id,
                Name = cartItem.Name,
                Image = new ItemImageDTO
                {
                    Url = cartItem.Image.Url,
                    AltText = cartItem.Image.AltText
                },
                Price = (decimal)cartItem.Price,
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

        public static CartItemMessage MapToMessage(CartItemDTO dto)
        {
            var message = new CartItemMessage
            {
                Id = dto.Id,
                Name = dto.Name,
                Image = new ItemImageMessage { AltText = dto.Image?.AltText, Url = dto.Image?.Url },
                Price = (double)dto.Price,
                Quantity = dto.Quantity
            };

            return message;
        }
    }

    
}
