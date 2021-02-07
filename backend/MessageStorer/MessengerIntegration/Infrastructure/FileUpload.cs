using MessengerIntegration.Config;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure
{
    public interface IFileUpload
    {
        string GetFilename(string importId);
        Task Upload(string importId, string base64content);
    }
    public class FileUpload: IFileUpload
    {
        private readonly IImportFileConfig _importFileConfig;

        public FileUpload(IImportFileConfig importFileConfig)
        {
            _importFileConfig = importFileConfig;
        }

        public string GetFilename(string importId)
        {
            return $"{_importFileConfig.Directory}/{importId}";
        }
        public async Task Upload(string importId, string base64content)
        {
            var filename = GetFilename(importId);
            using (var file = new FileStream(filename, FileMode.Append, FileAccess.Write))
            {
                var content = Convert.FromBase64String(base64content);
                await file.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
