using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class AppUsers
    {
        public AppUsers()
        {
            Contacts = new HashSet<Contacts>();
            Imports = new HashSet<Imports>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Contacts> Contacts { get; set; }
        public virtual ICollection<Imports> Imports { get; set; }
    }
}
