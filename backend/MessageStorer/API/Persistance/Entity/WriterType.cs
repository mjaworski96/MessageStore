using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class WriterType
    {
        public WriterType()
        {
            Message = new HashSet<Message>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Message> Message { get; set; }
    }
}
