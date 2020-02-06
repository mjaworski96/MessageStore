using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class AliasesMembers
    {
        public int Id { get; set; }
        public int AliasId { get; set; }
        public int ContactId { get; set; }

        public virtual Aliases Alias { get; set; }
        public virtual Contacts Contact { get; set; }
    }
}
