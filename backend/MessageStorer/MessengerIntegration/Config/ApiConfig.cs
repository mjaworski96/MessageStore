using Microsoft.Extensions.Configuration;

namespace MessengerIntegration.Config
{
    public interface IApiConfig
    {
        public string Token { get; }
    }
    public class ApiConfig : IApiConfig
    {
        public ApiConfig(IConfiguration configuration)
        {
            configuration.GetSection("Api").Bind(this);
        }
        public string Token { get; set; }
    }
}
