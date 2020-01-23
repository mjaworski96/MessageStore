using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Contact
    {
        public Contact()
        {
            AliasMember = new HashSet<AliasMember>();
            Message = new HashSet<Message>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int AppUserId { get; set; }
        public int ApplicationId { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<AliasMember> AliasMember { get; set; }
        public virtual ICollection<Message> Message { get; set; }
    }
}
