using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class MessageDto
    {
        public string Content { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public DateTime? Date { get; set; }
        //"app_user" or "contact"
        public string WriterType { get; set; }
        public int ContactId { get; set; }
    }
    public class MessageDtoWithId
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<AttachmentDtoWithId> Attachments { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public int ContactId { get; set; }
    }
}
