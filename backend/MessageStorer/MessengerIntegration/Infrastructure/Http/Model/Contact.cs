using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http.Model
{
    public class Contact
    {
        public string Name { get; set; }
        public string InApplicationId { get; set; }
        public List<ContactMember> Members { get; set; }
    }
    public class ContactWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InApplicationId { get; set; }
        public List<ContactMemberWithId> Members { get; set; }
    }
    public class ContactMember
    {
        public string Name { get; set; }
        public string InternalId { get; set; }
    }
    public class ContactMemberWithId
    {
        public int Id { get; set; }
        public string InternalId { get; set; }
        public string Name { get; set; }
    }
}
