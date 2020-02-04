using System;

namespace MessageSender.Model
{
    public class Sms
    {
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public string Contact { get; set; }
        public string Writer { get; set; }

        public const string WRITER_ME = "AppUser";
        public const string WRITER_CONTACT = "Contact";

    }
}
