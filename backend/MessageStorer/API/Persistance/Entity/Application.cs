using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Application
    {
        public Application()
        {
            Contact = new HashSet<Contact>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Contact> Contact { get; set; }
    }
}
