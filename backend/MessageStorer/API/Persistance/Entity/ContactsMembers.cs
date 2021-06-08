using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class ContactsMembers
    {
        public ContactsMembers()
        {
            Messages = new HashSet<Messages>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string InternalId { get; set; }
        public int? ContactId { get; set; }

        public virtual Contacts Contact { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
