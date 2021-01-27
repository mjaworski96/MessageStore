using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Common.Service
{
    public interface IHttpMetadataService
    {
        int UserId { get; }
        string Application { get; }
    }
    public class HttpMetadataService : IHttpMetadataService
    {
        private HttpContext _httpContext;
        public HttpMetadataService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }
        public int UserId
        {
            get
            {
                return int.Parse(_httpContext
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
                return _httpContext.Request.Headers["X-Application"];
            }
        }
    }
}
