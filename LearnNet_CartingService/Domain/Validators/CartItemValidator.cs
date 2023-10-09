using FluentValidation;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Domain.Validators
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.Quantity).NotEmpty();
        }
    }
}
