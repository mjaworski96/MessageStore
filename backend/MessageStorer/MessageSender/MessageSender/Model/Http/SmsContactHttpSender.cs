using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class SmsContactHttpSender: IDisposable
    {
        public const string ADDRESS = "http://10.8.39.172:5000";
        public const string URL = "api/messages";
        private HttpClient _http = new HttpClient();
        public SmsContactHttpSender()
        {
            _http = new HttpClient();
        }

        public void Dispose()
        {
            _http.Dispose();
        }

        public async Task<SmsWithId> Send(Sms sms)
        {
            try
            {
                var json = JsonConvert.SerializeObject(sms);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var result = _http.PostAsync($"{ADDRESS}/{URL}", data).Result;
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
