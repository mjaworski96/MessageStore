using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class ContactHttpSender : HttpSender
    {
        private const string URL = "api/contacts";

        public ContactHttpSender(string baseAddress) : base(baseAddress)
        {
        }

        public async Task<ContactWithId> Send(Contact contact)
        {
            var json = JsonConvert.SerializeObject(contact);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _http.PutAsync(URL, data);
            var body = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ContactWithId>(body);
        }
    }
}
