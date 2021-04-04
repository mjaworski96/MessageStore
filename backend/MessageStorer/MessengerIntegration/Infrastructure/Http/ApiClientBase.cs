using MessengerIntegration.Exceptions;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http
{
    public abstract class ApiClientBase
    {
        private SharedToken _authorizationToken;
        protected HttpClient _httpClient;

        protected ApiClientBase(SharedToken authorizationToken, HttpClient httpClient)
        {
            _authorizationToken = authorizationToken;
            _httpClient = httpClient;
        }
        protected async Task CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new ApiRequestException($"Request failed with code {response.StatusCode}: {responseBody}");
            }
            if (response.Headers.Contains("Authorization"))
            {
                _authorizationToken.AuthorizationToken = response.Headers.GetValues("Authorization").First();
            }
        }
        protected void AddAuthorization(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", _authorizationToken.AuthorizationToken);
        }
        protected async Task<TResult> Get<TResult>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddAuthorization(request);
            var response = await _httpClient.SendAsync(request);
            await CheckResponse(response);
            var resultBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(resultBody);
        }
        protected async Task Post<TBody>(string url, TBody body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            AddAuthorization(request);
            var requestBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            await CheckResponse(response);
        }

        protected async Task<TResult> Put<TResult, TBody>(string url, TBody body)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            AddAuthorization(request);
            var requestBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            await CheckResponse(response);
            var resultBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(resultBody);
        }
    }
}
