﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MessengerIntegration.Persistance.Entity
{
    public partial class Statuses
    {
        public Statuses()
        {
            Imports = new HashSet<Imports>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Imports> Imports { get; set; }
    }
}
