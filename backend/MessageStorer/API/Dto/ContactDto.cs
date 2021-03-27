using System.Collections.Generic;

namespace API.Dto
{
    public class ContactDto
    {
        public string Name { get; set; }
        public string InApplicationId { get; set; }
        public List<ContactMemberDto> Members { get; set; }
    }
    public class ContactDtoWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InApplicationId { get; set; }
        public List<ContactMemberWithIdDto> Members { get; set; }
    }
    public class ContactMemberDto
    {
        public string Name { get; set; }
    }
    public class ContactMemberWithIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
