using Microsoft.Extensions.Configuration;

namespace API.Config
{
    public interface ISecurityConfig
    {
        public string Key { get; }
        public int ValidFor { get; }
        public int RefreshBefore { get; }
    }
    public class SecurityConfig: ISecurityConfig
    {
        public SecurityConfig(IConfiguration configuration)
        {
            configuration.GetSection("Security").Bind(this);
        }
        public string Key { get; set; }
        public int ValidFor { get; set; }
        public int RefreshBefore { get; set; }
    }
}
