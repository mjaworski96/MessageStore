using API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                    var refreshedData = await appUserService
                        .Refresh(authorization.ToString());
                    if (refreshedData != null)
                    {
                        httpContext.Response.Headers.Add("Authorization", refreshedData.Token);
                        httpContext.Response.Headers.Add("X-User", 
                            JsonConvert.SerializeObject(refreshedData.AppUser,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
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
