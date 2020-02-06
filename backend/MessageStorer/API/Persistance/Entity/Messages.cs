using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Messages
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public byte[] Attachment { get; set; }
        public DateTime? Date { get; set; }
        public int WriterTypeId { get; set; }
        public int ContactId { get; set; }

        public virtual Contacts Contact { get; set; }
        public virtual WriterTypes WriterType { get; set; }
    }
}
