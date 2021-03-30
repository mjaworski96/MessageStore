using MessengerIntegration.Exceptions;
using System.Linq;
using System.Net.Http;
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
        protected async Task CheckResponseAsync(HttpResponseMessage response)
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
    }
}
