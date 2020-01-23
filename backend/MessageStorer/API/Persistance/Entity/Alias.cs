using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Alias
    {
        public Alias()
        {
            AliasMember = new HashSet<AliasMember>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Internal { get; set; }

        public virtual ICollection<AliasMember> AliasMember { get; set; }
    }
}
