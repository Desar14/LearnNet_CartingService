using FluentValidation;
using LearnNet_CartingService.Core.DTO;

namespace LearnNet_CartingService.Core.Validators
{
    public class CartItemDTOValidator : AbstractValidator<CartItemDTO>
    {
        public CartItemDTOValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.Quantity).NotEmpty().When(x => !x.Updating);
        }
    }
}
