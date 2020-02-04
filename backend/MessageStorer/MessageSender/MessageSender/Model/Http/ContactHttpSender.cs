using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class ContactHttpSender : IDisposable
    {
        public const string ADDRESS = "http://10.8.39.172:5000";
        public const string URL = "api/contacts";
        private HttpClient _http = new HttpClient();
        public ContactHttpSender()
        {
            _http = new HttpClient();
        }

        public void Dispose()
        {
            _http.Dispose();
        }

        public async Task<ContactWithId> Send(Contact contact)
        {
            try
            {
                var json = JsonConvert.SerializeObject(contact);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var result = _http.PutAsync($"{ADDRESS}/{URL}", data).Result;
                var body = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ContactWithId>(body);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
