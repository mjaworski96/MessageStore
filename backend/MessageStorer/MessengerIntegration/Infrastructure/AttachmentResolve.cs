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
        ZipArchiveEntry ResolveForPhoto(List<ZipArchiveEntry> entries, string uri);
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

        public ZipArchiveEntry ResolveForPhoto(List<ZipArchiveEntry> entries, string uri)
        {
            entries = entries.Where(x => x.FullName.Contains("photos")).ToList();
            var last = uri.Split("/").LastOrDefault();
            var filenamePrefix = last.Substring(0, last.IndexOf("."));
            if (string.IsNullOrEmpty(last))
            {
                _logger.LogWarning($"Can't resolve picture: {uri}");
                return null;
            }
            var entry = entries.FirstOrDefault(x => x.Name.StartsWith(filenamePrefix));
            if (entry == null)
            {
                _logger.LogWarning($"Can't resolve picture: {uri}");
            }
            return entry;
        }
    }
}
