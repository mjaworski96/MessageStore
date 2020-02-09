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
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
