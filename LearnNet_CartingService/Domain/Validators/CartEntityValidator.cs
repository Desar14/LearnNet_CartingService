using FluentValidation;
using LearnNet_CartingService.Domain.Entities;

namespace LearnNet_CartingService.Domain.Validators
{
    public class CartEntityValidator : AbstractValidator<CartEntity>
    {
        public CartEntityValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}
