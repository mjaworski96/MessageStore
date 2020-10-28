using Microsoft.Extensions.Configuration;

namespace API
{
    public interface ISecurityConfig
    {
        public string Key { get; }
        public int ValidFor { get; }
    }
    public class SecurityConfig: ISecurityConfig
    {
        public SecurityConfig(IConfiguration configuration)
        {
            configuration.GetSection("Security").Bind(this);
        }
        public string Key { get; set; }
        public int ValidFor { get; set; }
    }
}
