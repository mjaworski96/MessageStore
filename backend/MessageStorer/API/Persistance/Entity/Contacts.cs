using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Contacts
    {
        public Contacts()
        {
            AliasesMembers = new HashSet<AliasesMembers>();
            ContactsMembers = new HashSet<ContactsMembers>();
            Messages = new HashSet<Messages>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string InApplicationId { get; set; }
        public int AppUserId { get; set; }
        public int ApplicationId { get; set; }

        public virtual AppUsers AppUser { get; set; }
        public virtual Applications Application { get; set; }
        public virtual ICollection<AliasesMembers> AliasesMembers { get; set; }
        public virtual ICollection<ContactsMembers> ContactsMembers { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
