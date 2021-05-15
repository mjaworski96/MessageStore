using System;
using System.Collections.Generic;

namespace MessengerIntegration.Infrastructure.Http.Model
{
    public class Message
    {
        public string Content { get; set; }
        public List<Attachment> Attachments { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public string ImportId { get; set; }
        public int ContactId { get; set; }
        public int? ContactMemberId { get; set; }
    }
}
