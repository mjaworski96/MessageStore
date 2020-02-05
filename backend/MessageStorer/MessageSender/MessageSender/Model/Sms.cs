using Newtonsoft.Json;
using System;

namespace MessageSender.Model
{
    public class Sms
    {
        public string Content { get; set; }
        public byte[] Attachment { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public int ContactId { get; set; }
        [JsonIgnore]
        public string PhoneNumber { get; set; }

        public const string WRITER_ME = "app_user";
        public const string WRITER_CONTACT = "contact";

    }
    public class SmsWithId
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public byte[] Attachment { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public int ContactId { get; set; }

    }
}
