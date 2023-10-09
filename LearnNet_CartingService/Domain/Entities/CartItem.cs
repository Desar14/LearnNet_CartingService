using LearnNet_CartingService.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LearnNet_CartingService.Domain.Entities
{
	public class CartItem : BaseAuditableEntity
    {
        [Required]
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageText { get; set; }
        [Range(0.01, int.MaxValue, ErrorMessage = "Please enter a price bigger than {1}")]
        public decimal Price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a quantity bigger than {1}")]
        public int Quantity { get; set; }
	}
}
