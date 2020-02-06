using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class SmsContactHttpSender: HttpSender
    {
        private const string URL = "api/messages";

        public async Task<SmsWithId> Send(Sms sms)
        {
            try
            {
                var json = JsonConvert.SerializeObject(sms);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _http.PostAsync(URL, data);
                var body = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SmsWithId>(body);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
