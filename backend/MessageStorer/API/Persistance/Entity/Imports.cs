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
        public bool IsBeingDeleted { get; set; }
        public int AppUserId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual AppUsers AppUser { get; set; }
        public virtual Applications Application { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
