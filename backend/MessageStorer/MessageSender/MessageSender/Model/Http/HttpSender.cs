using System;
using System.Net.Http;

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

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
