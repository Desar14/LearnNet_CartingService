using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using LearnNet_CartingService.Core.Interfaces;
using Asp.Versioning;

namespace LearnNet_CartingService.Controllers.v0
{
    [ApiVersion(0.1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IValidator<CartItemDTO> _validator;

        public CartItemsController(ICartService cartService, IValidator<CartItemDTO> validator)
        {
            _cartService = cartService;
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
        public async Task<IEnumerable<CartItemDTO>> GetAsync(string id)
        {
            return await _cartService.GetAllCartItemsAsync(id);
        }

        // POST api/<CartItemsController>
        [HttpPost]
        public async Task<IActionResult> Post(string id, [FromBody] CartItemDTO cartItemDTO)
        {
            var validationResult = await _validator.ValidateAsync(cartItemDTO);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem();
            }

            var result = await _cartService.AddCartItemAsync(id, cartItemDTO);

            return result ? Ok() : BadRequest();
        }

        // PUT api/<CartItemsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] string value)
        {
            return NotFound();
        }

        // DELETE api/<CartItemsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, int cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(id, cartItemId);

            return result ? Ok() : BadRequest();
        }
    }
}
