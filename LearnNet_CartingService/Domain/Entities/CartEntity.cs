using FluentValidation;
using LearnNet_CartingService.Domain.Common;

namespace LearnNet_CartingService.Domain.Entities
{
    public class CartEntity : BaseAuditableEntity
    {
        public IList<CartItem> Items { get; private set; } = new List<CartItem>();
    }
}
