using System.Collections.Generic;

namespace API.Dto
{
    public class AliasDtoWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Internal { get; set; }
        public string Application { get; set; }
        public string InApplicationId { get; set; }
        public List<AliasMemberDtoWithId> Members { get; set; }
    }
    public class AliasDtoWithIdList
    {
        public List<AliasDtoWithId> Aliases { get; set; }
    }
    public class AliasMemberDtoWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Application { get; set; }
        public string InApplicationId { get; set; }
        public List<ContactMemberWithIdDto> ContactMembers { get; set; }
    }
    public class CreateAliasDto
    {
        public string Name { get; set; }
        public List<IdDto> Members { get; set; }
    }
}
