using FluentValidation;
using LearnNet_CartingService.Domain.Common;

namespace LearnNet_CartingService.Domain.Entities
{
    public class CartEntity : BaseAuditableEntity<string>
    {
        
        public IList<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
