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

    public class CartItemDTOValidator : AbstractValidator<CartItemDTO>
	{
		public CartItemDTOValidator()
		{
			RuleFor(x => x.Id).NotNull();
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.Price).NotEmpty();
			RuleFor(x => x.Quantity).NotEmpty();
		}
	}
}
