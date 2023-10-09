using FluentValidation;

namespace LearnNet_CartingService.Core.DTO
{
	public class CartItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageText { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
