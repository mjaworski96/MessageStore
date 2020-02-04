namespace API.Dto
{
    public class ContactDto
    {
        public string Name { get; set; }
        public string InApplicationId { get; set; }
    }
    public class ContactDtoWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InApplicationId { get; set; }
    }
}
