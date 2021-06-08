using System;
using System.Collections.Generic;

namespace API.Persistance.Entity
{
    public partial class Imports
    {
        public Imports()
        {
            Messages = new HashSet<Messages>();
        }

        public int Id { get; set; }
        public string ImportId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
    }
}
