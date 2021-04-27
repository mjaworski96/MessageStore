using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class AddTokenFromQueryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RefreshTokenMiddleware> _logger;

        public AddTokenFromQueryMiddleware(RequestDelegate next, ILogger<RefreshTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                if (httpContext.Request.Query.ContainsKey("access_token"))
                {
                    var token = httpContext.Request.Query["access_token"];
                    httpContext.Request.Headers.Add("Authorization", token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while routing token: {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
            }
            await _next(httpContext);
        }
    }
}
