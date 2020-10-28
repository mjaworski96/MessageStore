using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace API.Service
{
    public interface IHttpMetadataService
    {
        string Username { get; }
        string Application { get; }
    }
    public class HttpMetadataService : IHttpMetadataService
    {
        private HttpContext _httpContext;
        public HttpMetadataService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }
        public string Username
        {
            get
            {
                return _httpContext
                    .User
                    .Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                    .Value;
            }
        }

        public string Application
        {
            get
            {
                return _httpContext.Request.Headers["X-Application"];
            }
        }
    }
}
