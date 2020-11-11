using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class HttpSender: IDisposable
    {
        protected HttpClient _http;
        public HttpSender(string baseAddress)
        {
            var session = new SessionStorage();
            var token = session.GetToken().Result;
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Application", "sms");
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Add("Authorization", token);
            }
            _http.BaseAddress = new Uri(baseAddress);
        }
        private async Task UpdateAuthorization()
        {
            var session = new SessionStorage();
            var token = await session.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                if(_http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _http.DefaultRequestHeaders.Remove("Authorization");
                }
                _http.DefaultRequestHeaders.Add("Authorization", token);
            }
        }
        public async Task CheckResponse(HttpResponseMessage message)
        {
            if(message.Headers.Contains("Authorization") &&
               message.Headers.Contains("X-User"))
            {
                var session = new SessionStorage();
                var jsonUser = message.Headers.GetValues("X-User").FirstOrDefault();
                var user = JsonConvert.DeserializeObject<LoggedUser>(jsonUser);
                await session.StoreSession(
                    message.Headers.GetValues("Authorization").FirstOrDefault(),
                    user);
                await UpdateAuthorization();
            }
            if (!message.IsSuccessStatusCode)
            {
                var body = await message.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ApiErrorResponse>(body);
                throw new ApiException(error);
            }
        }
        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
