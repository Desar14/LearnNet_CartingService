using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LearnNet_CartingService.Auth
{
    public class AuthLogMiddleware : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        private readonly ILogger _logger;

        public AuthLogMiddleware(ILoggerFactory loggerFactory)
            => _logger = loggerFactory.CreateLogger(GetType().FullName);

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            var claims = new List<ClaimModel>();

            foreach (var item in context.User.Claims)
            {
                var model = new ClaimModel()
                {
                    _issuer = item.Issuer,
                    _type = item.Type,
                    _value = item.Value,
                    _valueType = item.ValueType,
                };

                claims.Add(model);
            }

            var claimsJson = JsonSerializer.Serialize(claims);
            
            _logger.LogInformation("User claims: " + claimsJson);
            
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
