using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Core.DTO
{
    public class CartDTO
    {
        public string Id { get; set; }
        public IList<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

        public static CartDTO MapFrom(CartEntity entity)
        {
            var dto = new CartDTO
            {
                Id = entity.Id,
                Items = entity.Items.Select(CartItemDTO.MapFrom).ToList()
            };

            return dto;
        }

        public static CartEntity MapTo(CartDTO dto)
        {
            var entity = new CartEntity
            {
                Id = dto.Id,
                Items = dto.Items.Select(CartItemDTO.MapTo).ToList()
            };

            return entity;
        }
    }
}
