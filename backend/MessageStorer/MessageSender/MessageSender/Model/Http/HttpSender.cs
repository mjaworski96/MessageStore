using System;
using System.Net.Http;

namespace MessageSender.Model.Http
{
    public class HttpSender: IDisposable
    {
        private const string ADDRESS = "http://10.8.39.172:5000";
        protected HttpClient _http;
        public HttpSender()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Application", "sms");
            _http.DefaultRequestHeaders.Add("X-MockedAuthority", "test");
            _http.BaseAddress = new Uri(ADDRESS);
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
