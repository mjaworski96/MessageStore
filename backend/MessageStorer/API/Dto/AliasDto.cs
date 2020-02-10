using System.Collections.Generic;

namespace API.Dto
{
    public class AliasDtoWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Internal { get; set; }
        public string InApplicationId { get; set; }
    }
    public class AliasDtoWithIdList
    {
        public List<AliasDtoWithId> Aliases { get; set; }
    }
}
