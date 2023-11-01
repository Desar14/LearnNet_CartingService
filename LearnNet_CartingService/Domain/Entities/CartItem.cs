using LearnNet_CartingService.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace LearnNet_CartingService.Domain.Entities
{
	public class CartItem : BaseAuditableEntity<int>
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageText { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
	}
}
