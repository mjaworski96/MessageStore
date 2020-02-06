using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Applications
    {
        public Applications()
        {
            Contacts = new HashSet<Contacts>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Contacts> Contacts { get; set; }
    }
}
