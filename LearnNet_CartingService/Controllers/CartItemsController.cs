using AutoMapper;
using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using LearnNet_CartingService.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnNet_CartingService.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IValidator<CartItemDTO> _validator;

		public CartItemsController(ICartService cartService, IMapper mapper, IValidator<CartItemDTO> validator)
		{
			_cartService = cartService;
            _mapper = mapper;
            _validator = validator;
		}

		// GET: api/<CartItemsController>
		[HttpGet]
        public async Task<IActionResult> Get()
        {
            return NotFound();
        }

        // GET api/<CartItemsController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<CartItemDTO>> GetAsync(int id)
        {
            return await _cartService.GetAllCartItemsAsync(id);
        }

        // POST api/<CartItemsController>
        [HttpPost]
        public async Task<IActionResult> Post(int id, [FromBody] CartItemDTO cartItemDTO)
		{
			var validationResult = await _validator.ValidateAsync(cartItemDTO);

			if (!validationResult.IsValid)
			{
				foreach (var error in validationResult.Errors)
				{
					this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				}

				return ValidationProblem();
			}
			
            var result = await _cartService.AddCartItemAsync(id, cartItemDTO);

			return result ? Ok() : BadRequest();
		}

		// PUT api/<CartItemsController>/5
		[HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            return NotFound();
        }

        // DELETE api/<CartItemsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, int cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(id, cartItemId);

            return result ? Ok() : BadRequest();
        }
    }
}
