using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class AliasMember
    {
        public int Id { get; set; }
        public int AliasId { get; set; }
        public int ContactId { get; set; }

        public virtual Alias Alias { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
