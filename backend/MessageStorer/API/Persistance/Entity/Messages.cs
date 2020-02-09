using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Messages
    {
        public Messages()
        {
            Attachments = new HashSet<Attachments>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public int WriterTypeId { get; set; }
        public int ContactId { get; set; }

        public virtual Contacts Contact { get; set; }
        public virtual WriterTypes WriterType { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
    }
}
