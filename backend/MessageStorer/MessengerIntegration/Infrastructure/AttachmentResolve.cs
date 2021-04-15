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
        ZipArchiveEntry ResolveForVideo(List<ZipArchiveEntry> entries, string uri);
        ZipArchiveEntry ResolveForGif(List<ZipArchiveEntry> entries, string uri);
        ZipArchiveEntry ResolveForAudio(List<ZipArchiveEntry> entries, string uri);
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
            return ResolveForPhotoGif("photos", entries, uri, "photo");
        }

        public ZipArchiveEntry ResolveForVideo(List<ZipArchiveEntry> entries, string uri)
        {
            return ResolveForVideoAudio("videos", entries, uri, "video");
        }

        public ZipArchiveEntry ResolveForGif(List<ZipArchiveEntry> entries, string uri)
        {
            return ResolveForPhotoGif("gifs", entries, uri, "gif");
        }
        public ZipArchiveEntry ResolveForPhotoGif(string directory, List<ZipArchiveEntry> entries, string uri, string type)
        {
            entries = entries.Where(x => x.FullName.Contains(directory)).ToList();
            var last = uri.Split("/").LastOrDefault();
            var filenamePrefix = last.Substring(0, last.IndexOf("."));
            if (string.IsNullOrEmpty(last))
            {
                _logger.LogWarning($"Can't resolve {type}: {uri}");
                return null;
            }
            var entry = entries.FirstOrDefault(x => x.Name.StartsWith(filenamePrefix));
            if (entry == null)
            {
                _logger.LogWarning($"Can't resolve {type}: {uri}");
            }
            return entry;
        }

        public ZipArchiveEntry ResolveForAudio(List<ZipArchiveEntry> entries, string uri)
        {
            return ResolveForVideoAudio("audio", entries, uri, "audio");
        }
        private ZipArchiveEntry ResolveForVideoAudio(string directory, List<ZipArchiveEntry> entries, string uri, string type)
        {
            entries = entries.Where(x => x.FullName.Contains(directory)).ToList();
            var last = uri.Split("/").LastOrDefault();
            var filenamePrefix = last.Substring(0, last.IndexOf(".")).Replace("-", "");
            if (string.IsNullOrEmpty(last))
            {
                _logger.LogWarning($"Can't resolve {type}: {uri}");
                return null;
            }
            var entry = entries.FirstOrDefault(x => x.Name.StartsWith(filenamePrefix));
            if (entry == null)
            {
                _logger.LogWarning($"Can't resolve {type}: {uri}");
            }
            return entry;
        }
    }
}
