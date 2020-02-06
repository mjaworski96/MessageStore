using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class WriterTypes
    {
        public WriterTypes()
        {
            Messages = new HashSet<Messages>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
    }
}
