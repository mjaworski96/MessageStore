using Microsoft.Extensions.Configuration;

namespace API.Config
{
    public interface IApkConfig
    {
        public string Filename { get; }
    }
    public class ApkConfig : IApkConfig
    {
        public ApkConfig(IConfiguration configuration)
        {
            configuration.GetSection("Apk").Bind(this);
        }
        public string Filename { get; set; }
    }
}
