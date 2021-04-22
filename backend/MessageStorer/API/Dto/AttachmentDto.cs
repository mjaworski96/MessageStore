using System.IO;

namespace API.Dto
{
    public class AttachmentDto
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
    public class AttachmentDtoWithId
    {
        public int Id { get; set; }
        public string ContentType { get; set; }
    }
    public class AttachmentContentDto
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
