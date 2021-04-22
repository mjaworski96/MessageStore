using Microsoft.Extensions.Configuration;

namespace API.Config
{
    public interface IAttachmentConfig
    {
        public string Directory { get; set; }
    }
    public class AttachmentConfig : IAttachmentConfig
    {
        public AttachmentConfig(IConfiguration configuration)
        {
            configuration.GetSection("Attachments").Bind(this);
        }

        public string Directory { get; set; }
    }
}
