using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Common.Service
{
    public interface IHttpMetadataService
    {
        int UserId { get; }
        string Application { get; }
        string AuthorizationToken { get; }
        bool InternalToken { get; }
    }
    public class HttpMetadataService : IHttpMetadataService
    {
        private IHttpContextAccessor _httpContextAccessor;
        public HttpMetadataService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int UserId
        {
            get
            {
                return int.Parse(_httpContextAccessor.HttpContext
                    .User
                    .Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                    .Value);
            }
        }

        public string Application
        {
            get
            {
                return _httpContextAccessor.HttpContext.Request.Headers["X-Application"];
            }
        }

        public string AuthorizationToken
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    return _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                }
                return string.Empty;
            }
        }

        public bool InternalToken
        {
            get
            {
                return bool.Parse(_httpContextAccessor.HttpContext
                    .User
                    .Claims
                    .FirstOrDefault(x => x.Type == "int")
                    ?.Value ?? "false");
            }
        }
    }
}
