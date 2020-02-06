using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Aliases
    {
        public Aliases()
        {
            AliasesMembers = new HashSet<AliasesMembers>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Internal { get; set; }

        public virtual ICollection<AliasesMembers> AliasesMembers { get; set; }
    }
}
