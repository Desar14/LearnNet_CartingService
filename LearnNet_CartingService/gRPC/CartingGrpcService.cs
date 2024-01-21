using Azure.Core;
using FluentValidation;
using Grpc.Core;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Core.Validators;
using Microsoft.OpenApi.Validations;
using static LearnNet_CartingService.gRPC.CartingGrpc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearnNet_CartingService.gRPC
{
    public class CartingGrpcService(ICartService cartService, IValidator<CartItemDTO> validator, ILogger<CartingGrpcService> logger) : CartingGrpcBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly IValidator<CartItemDTO> _validator = validator;
        private readonly ILogger<CartingGrpcService> _logger = logger;

        public override async Task AddItemsBiStream(IAsyncStreamReader<AddCartItemsStreamRequest> requestStream, IServerStreamWriter<CartItemStreamResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Processing stream add cart items to cart {message.CartId}");

                var cartItemDTO = CartItemDTO.MapFrom(message.Item);

                var validationResult = await _validator.ValidateAsync(cartItemDTO);

                if (!validationResult.IsValid)
                {
                    string errors = "";
                    foreach (var error in validationResult.Errors)
                    {
                        errors += $"Field {error.PropertyName} has error {error.ErrorMessage}\n";
                    }

                    throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
                }

                _logger.LogInformation($"Cart item to cart {message.CartId} with id = {cartItemDTO.Id} validated successfully.");

                if (!(await _cartService.AddCartItemAsync(message.CartId, cartItemDTO)))
                {
                    throw new RpcException(new Status(StatusCode.DataLoss, "Adding cart item failed"));
                }

                _logger.LogInformation($"Cart items to cart {message.CartId} added successfully. Returning full cart.");

                var result = await _cartService.GetAllCartItemsAsync(message.CartId);

                if (result == null) // would be very strange
                {
                    _logger.LogWarning($"Requested cart with id = {message.CartId} not found");
                    throw new RpcException(new Status(StatusCode.NotFound, "Cart not found."));
                }

                var response = new CartItemStreamResponse
                {
                    CartId = message.CartId
                };

                foreach (var item in result)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        throw new RpcException(new Status(StatusCode.Aborted, "Cancellation token called")); ;
                    }

                    response.Items.Add(CartItemDTO.MapToMessage(item));
                }

                _logger.LogInformation($"Sending Cart items with id = {response.CartId} in unary response");

                await responseStream.WriteAsync(response);
            }

        }

        public override async Task<GetCartItemsUnaryResponse> AddItemsStream(IAsyncStreamReader<AddCartItemsStreamRequest> requestStream, ServerCallContext context)
        {
            string cartId = "";
            List<CartItemDTO> cartItemDTOs = new();

            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Processing stream add cart items to cart {message.CartId}");

                if (string.IsNullOrEmpty(cartId))
                {
                    cartId = message.CartId;
                }
                else if (cartId != message.CartId)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "You can't send different cart ids in one RPC call."));
                }

                var cartItemDTO = CartItemDTO.MapFrom(message.Item);

                var validationResult = await _validator.ValidateAsync(cartItemDTO);

                if (!validationResult.IsValid)
                {
                    string errors = "";
                    foreach (var error in validationResult.Errors)
                    {
                        errors += $"Field {error.PropertyName} has error {error.ErrorMessage}\n";
                    }

                    throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
                }

                cartItemDTOs.Add(cartItemDTO);

                _logger.LogInformation($"Cart item to cart {message.CartId} with id = {cartItemDTO.Id} validated successfully.");
            }

            foreach (var item in cartItemDTOs)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw new RpcException(new Status(StatusCode.Aborted, "Cancellation token called")); ;
                }

                if (!(await _cartService.AddCartItemAsync(cartId, item)))
                {
                    throw new RpcException(new Status(StatusCode.DataLoss, "Adding cart item failed"));
                }
            }

            _logger.LogInformation($"Cart items to cart {cartId} added successfully. Returning full cart.");

            var result = await _cartService.GetAllCartItemsAsync(cartId);

            if (result == null) // would be very strange
            {
                _logger.LogWarning($"Requested cart with id = {cartId} not found");
                throw new RpcException(new Status(StatusCode.NotFound, "Cart not found."));
            }

            var response = new GetCartItemsUnaryResponse();

            foreach (var item in result)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw new RpcException(new Status(StatusCode.Aborted, "Cancellation token called")); ;
                }

                response.Items.Add(CartItemDTO.MapToMessage(item));
            }

            _logger.LogInformation($"Sending Cart items with id = {cartId} in unary response");

            return response;
        }

        public override async Task GetItemsStream(GetCartItemsByIdRequest request, IServerStreamWriter<CartItemStreamResponse> responseStream, ServerCallContext context)
        {
            var result = await _cartService.GetAllCartItemsAsync(request.CartId);

            _logger.LogInformation($"Cart items with cart id = {request.CartId} requested in server stream call");

            if (result == null)
            {
                _logger.LogWarning($"Requested cart with id = {request.CartId} not found");
                throw new RpcException(new Status(StatusCode.NotFound, "Cart not found."));
            }

            _logger.LogInformation($"Sending Cart items with cart id = {request.CartId} in stream call starting");

            foreach (var item in result)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw new RpcException(new Status(StatusCode.Aborted, "Cancellation token called")); ;
                }

                var response = new CartItemStreamResponse
                {
                    CartId = request.CartId
                };

                response.Items.Add(CartItemDTO.MapToMessage(item));

                _logger.LogInformation($"Sending Cart item with id = {item.Id} in stream response");

                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            _logger.LogInformation($"Sending Cart items with cart id = {request.CartId} in stream response finished");
        }

        public override async Task<GetCartItemsUnaryResponse> GetItemsUnary(GetCartItemsByIdRequest request, ServerCallContext context)
        {
            var result = await _cartService.GetAllCartItemsAsync(request.CartId);

            _logger.LogInformation($"Cart items with id = {request.CartId} requested in unary call");

            if (result == null)
            {
                _logger.LogWarning($"Requested cart with id = {request.CartId} not found");
                throw new RpcException(new Status(StatusCode.NotFound, "Cart not found."));
            }

            var response = new GetCartItemsUnaryResponse();

            foreach (var item in result)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw new RpcException(new Status(StatusCode.Aborted, "Cancellation token called")); ;
                }

                response.Items.Add(CartItemDTO.MapToMessage(item));
            }

            _logger.LogInformation($"Sending Cart items with id = {request.CartId} in unary response");

            return response;
        }
    }
}
