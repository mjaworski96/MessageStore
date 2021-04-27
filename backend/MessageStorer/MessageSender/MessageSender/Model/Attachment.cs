namespace MessageSender.Model
{
    public class Attachment
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string SaveAsFilename { get; set; }
    }
    public class AttachmentWithId
    {
        public int Id { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
