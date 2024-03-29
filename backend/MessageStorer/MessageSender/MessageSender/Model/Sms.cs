﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MessageSender.Model
{
    public class Sms
    {
        public string Content { get; set; }
        public List<Attachment> Attachments { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public bool HasError { get; set; }
        public string ImportId { get; set; }
        public int ContactId { get; set; }
        public int? ContactMemberId { get; set; }
        [JsonIgnore]
        public int? Person { get; set; }
        [JsonIgnore]
        public string ThreadId { get; set; }
        [JsonIgnore]
        public bool ShouldBeIgnored { get; set; }

        public const string WRITER_ME = "app_user";
        public const string WRITER_CONTACT = "contact";

    }
    public class SmsWithId
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<AttachmentWithId> Attachments { get; set; }
        public DateTime? Date { get; set; }
        public string WriterType { get; set; }
        public int ContactId { get; set; }

    }
}
