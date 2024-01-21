using Azure.Core;
using FluentValidation;
using Grpc.Core;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using static LearnNet_CartingService.gRPC.CartingGrpc;

namespace LearnNet_CartingService.gRPC
{
    public class CartingGrpcService(ICartService cartService, IValidator<CartItemDTO> validator, ILogger<CartingGrpcService> logger) : CartingGrpcBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly IValidator<CartItemDTO> _validator = validator;
        private readonly ILogger<CartingGrpcService> _logger = logger;

        public override Task AddItemsBiStream(IAsyncStreamReader<AddCartItemsStreamRequest> requestStream, IServerStreamWriter<CartItemStreamResponse> responseStream, ServerCallContext context)
        {
            return base.AddItemsBiStream(requestStream, responseStream, context);
        }

        public override Task<GetCartItemsUnaryResponse> AddItemsStream(IAsyncStreamReader<AddCartItemsStreamRequest> requestStream, ServerCallContext context)
        {
            
            return base.AddItemsStream(requestStream, context);
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
                    Item = new CartItemMessage
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = decimal.ToDouble(item.Price),
                        Quantity = item.Quantity,
                        Image = new ItemImageMessage { Url = item.Image?.Url, AltText = item.Image?.AltText }
                    }
                };

                _logger.LogInformation($"Sending Cart item with id = {item.Id} in stream call");

                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            _logger.LogInformation($"Sending Cart items with cart id = {request.CartId} in stream call finished");
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

                response.Items.Add(new CartItemMessage
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = decimal.ToDouble(item.Price),
                    Quantity = item.Quantity,
                    Image = new ItemImageMessage { Url = item.Image?.Url, AltText = item.Image?.AltText }
                }
                );
            }

            _logger.LogInformation($"Sending Cart items with id = {request.CartId} in unary call");

            return response;
        }
    }
}
