using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class HttpSender: IDisposable
    {
        protected HttpClient _http;
        public HttpSender(string baseAddress)
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Application", "sms");
            _http.DefaultRequestHeaders.Add("X-MockedAuthority", "test");
            _http.BaseAddress = new Uri(baseAddress);
        }
        public async Task CheckResponse(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                var exception = JsonConvert.DeserializeObject<ApiException>(
                    await message.Content.ReadAsStringAsync());
                throw exception;
            }
        }
        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
