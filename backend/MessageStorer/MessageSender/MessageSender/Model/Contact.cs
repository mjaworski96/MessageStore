using Newtonsoft.Json;

namespace MessageSender.Model
{
    public class Contact
    {
        public string Name { get; set; }
        [JsonProperty("inApplicationId")]
        public string PhoneNumber { get; set; }
    }
    public class ContactWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("inApplicationId")]
        public string PhoneNumber { get; set; }
    }
}
