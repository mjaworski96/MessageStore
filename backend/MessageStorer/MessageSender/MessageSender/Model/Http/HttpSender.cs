using Javax.Net.Ssl;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    internal class BypassHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            var publicKey = session.GetPeerCertificates()[0].PublicKey.ToString();
            return publicKey == $"OpenSSLRSAPublicKey{{modulus={Constraints.PublicKey},publicExponent=10001}}";
        }
    }

    internal class BypassSslValidationClientHandler : Xamarin.Android.Net.AndroidClientHandler
    {
        private readonly SSLSocketFactory _factory;
        private readonly IHostnameVerifier _hostnameVerifier;

        internal BypassSslValidationClientHandler()
        {
            _factory = Android.Net.SSLCertificateSocketFactory.GetInsecure(60000, null);
            _hostnameVerifier = new BypassHostnameVerifier();
        }
        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
            return _factory;
        }

        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return _hostnameVerifier;
        }
    }

    public class HttpSender : IDisposable
    {
        protected HttpClient _http;
        public HttpSender(string baseAddress)
        {
            var session = new SessionStorage();
            var token = session.GetToken().Result;
            var handler = new BypassSslValidationClientHandler();
            _http = new HttpClient(handler);
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
                if (_http.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _http.DefaultRequestHeaders.Remove("Authorization");
                }
                _http.DefaultRequestHeaders.Add("Authorization", token);
            }
        }
        public async Task CheckResponse(HttpResponseMessage message)
        {
            if (message.Headers.Contains("Authorization") &&
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
                if (error == null &&
                    message.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ApiException(401);
                }
                throw new ApiException(error);
            }
        }
        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
