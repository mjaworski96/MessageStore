using Microsoft.Extensions.Configuration;

namespace API.Config
{
    public interface ISecurityConfig
    {
        public string Key { get; }
        public int ValidFor { get; }
        public int RefreshAfter { get; }
        public int InternalValidFor { get; }
        public int InternalRefreshAfter { get; }
        public string InternalToken { get; }
    }
    public class SecurityConfig: ISecurityConfig
    {
        public SecurityConfig(IConfiguration configuration)
        {
            configuration.GetSection("Security").Bind(this);
        }
        public string Key { get; set; }
        public int ValidFor { get; set; }
        public int RefreshAfter { get; set; }
        public int InternalValidFor { get; set; }
        public int InternalRefreshAfter { get; set; }
        public string InternalToken { get; set; }
    }
}
