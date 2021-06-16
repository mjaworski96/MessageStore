using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure
{
    public interface IAttachmentResolve
    {
        ZipArchiveEntry Resolve(List<ZipArchiveEntry> entries, string uri);
        string GetMimeType(string path);
    }
    public class AttachmentResolve : IAttachmentResolve
    {
        private readonly ILogger<AttachmentResolve> _logger;

        public AttachmentResolve(ILogger<AttachmentResolve> logger)
        {
            _logger = logger;
        }

        public string GetMimeType(string path)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);
            return contentType;
        }

        public ZipArchiveEntry Resolve(List<ZipArchiveEntry> entries, string uri)
        {
            var entry = entries.FirstOrDefault(x => x.FullName == uri);
            if (entry == null)
            {
                _logger.LogWarning($"Can't resolve attachment: {uri}");
            }
            return entry;
        }
    }
}
