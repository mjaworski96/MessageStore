using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class AppUser
    {
        public AppUser()
        {
            Contact = new HashSet<Contact>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Contact> Contact { get; set; }
    }
}
