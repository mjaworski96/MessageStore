using API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RefreshTokenMiddleware> _logger;

        public RefreshTokenMiddleware(RequestDelegate next, ILogger<RefreshTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext,
            IAppUserService appUserService)
        {
            try
            {
                if (httpContext.Request.Headers
                        .TryGetValue("Authorization", out var authorization) &&
                    !httpContext.Response.Headers.ContainsKey("Authorization"))
                {
                    var refreshedToken = appUserService
                        .RefreshToken(authorization.ToString());
                    if (!string.IsNullOrEmpty(refreshedToken))
                    {
                        httpContext.Response.Headers.Add("Authorization", refreshedToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while refreshing token: {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
            }
            await _next(httpContext);
        }
    }
}
