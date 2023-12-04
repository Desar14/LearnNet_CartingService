using Asp.Versioning;
using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnNet_CartingService.Controllers
{
    /// <summary>
    /// Represents a RESTful service of Carts.
    /// </summary>
    [Authorize]
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

        /// <summary>
        /// Gets a cart with its items.
        /// </summary>
        /// <param name="id">The requested cart identifier.</param>
        /// <returns>The requested cart.</returns>
        /// <response code="200">The cart was successfully retrieved.</response>
        /// <response code="404">The cart does not exist.</response>
        [HttpGet("{id}")]
        [ApiVersion(1.0)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var result = await _cartService.GetCartAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets an items of cart.
        /// </summary>
        /// <param name="id">The requested cart identifier.</param>
        /// <returns>The requested cart.</returns>
        /// <response code="200">The cart was successfully retrieved.</response>
        /// <response code="404">The cart does not exist.</response>
        [HttpGet("{id}")]
        [ApiVersion(2.0)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<CartItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetV2Async(string id)
        {
            var result = await _cartService.GetAllCartItemsAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Adds a new item to cart. If cart not exists - creates it.
        /// </summary>
        /// <param name="cartId">The cart to modify.</param>
        /// <param name="cartItemDTO">The item to add.</param>
        /// <returns>The created order.</returns>
        /// <response code="200">The item was successfully added.</response>
        /// <response code="400">The item is invalid.</response>
        [ApiVersion(1.0)]
        [ApiVersion(2.0)]
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(string cartId, [FromBody] CartItemDTO cartItemDTO)
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

            var result = await _cartService.AddCartItemAsync(cartId, cartItemDTO);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Updated properties of item in all carts.
        /// </summary>
        /// <param name="cartItemDTO">The item to update.</param>
        /// <returns>The created order.</returns>
        /// <response code="200">The item was successfully added, or not found in any cart.</response>
        /// <response code="400">The item is invalid.</response>
        [ApiVersion(1.0)]
        [ApiVersion(2.0)]
        [HttpPut]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] CartItemDTO cartItemDTO)
        {
            cartItemDTO.Updating = true;
            cartItemDTO.Quantity = -1; //doesn't matter here

            var validationResult = await _validator.ValidateAsync(cartItemDTO);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem();
            }

            var result = await _cartService.UpdateItemsAsync(cartItemDTO);

            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Delete an Item from cart. If it's the last item - cart would be deleted also.
        /// </summary>
        /// <param name="id">The item of cart to delete.</param>
        /// <param name="cartItemId">The cart to modify.</param>
        /// <returns>The created order.</returns>
        /// <response code="200">The item was successfully deleted.</response>
        /// <response code="400">The item is invalid.</response>
        [ApiVersion(1.0)]
        [ApiVersion(2.0)]
        [HttpDelete("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id, int cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(id, cartItemId);

            return result ? Ok() : BadRequest();
        }
    }
}
