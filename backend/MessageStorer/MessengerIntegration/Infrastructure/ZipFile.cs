using MessengerIntegration.Persistance.Entity;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure
{
    public interface IZipFile
    {
        FileStream Open(Imports imports);
        ZipArchive Open(FileStream fileStream);
        Dictionary<string, List<ZipArchiveEntry>> GetMessages(ZipArchive zip);
        Stream GetConversationStream(List<ZipArchiveEntry> entries);
        Task<JsonDocument> GetConversation(Stream conversationStream);
    }
    public class ZipFile : IZipFile
    {
        private readonly IFileUtils _fileUtils;

        public ZipFile(IFileUtils fileUtils)
        {
            _fileUtils = fileUtils;
        }
        public FileStream Open(Imports imports)
        {
            string filename = _fileUtils.GetFilename(imports.Id);
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }
        public ZipArchive Open(FileStream fileStream)
        {
            return new ZipArchive(fileStream, ZipArchiveMode.Read);
        }
        public Dictionary<string, List<ZipArchiveEntry>> GetMessages(ZipArchive zip)
        {
            return zip.Entries
                .Where(x => x.FullName.Contains("messages/inbox/") && x.FullName.Split("/").Length > 3)
                .GroupBy(x => x.FullName.Split("/")[2])
                .ToDictionary(key => key.Key, value => value.ToList());
        }

        public Stream GetConversationStream(List<ZipArchiveEntry> entries)
        {
            var entry = entries.FirstOrDefault(x => x.FullName.EndsWith("message_1.json"));
            if (entry != null)
            {
                return entry.Open();
            }
            return null;
        }
        public async Task<JsonDocument> GetConversation(Stream conversationStream)
        {
            var document = await JsonDocument.ParseAsync(conversationStream);
            return document;
        }
    }
}
