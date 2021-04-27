using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Attachments
    {
        public int Id { get; set; }
        public string ContentType { get; set; }
        public int MessageId { get; set; }
        public string Filename { get; set; }
        public string SaveAsFilename { get; set; }

        public virtual Messages Message { get; set; }
    }
}
