using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MessengerIntegration.Persistance.Entity
{
    public partial class Imports
    {
        public string Id { get; set; }
        public int StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }
        public string FbUsername { get; set; }

        public virtual Statuses Status { get; set; }
    }
}
