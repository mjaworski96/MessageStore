using MessengerIntegration.Config;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http
{
    public class UserApiClient : ApiClientBase
    {
        private readonly IApiConfig _apiConfig;
        public UserApiClient(SharedToken authorizationToken, HttpClient httpClient, IApiConfig apiConfig) : base(authorizationToken, httpClient)
        {
            _apiConfig = apiConfig;
        }
        public async Task Authorize(int userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/internal/genereateInternalToken?forUser={userId}");
            request.Headers.Add("Authorization", _apiConfig.Token);
            var response = await _httpClient.SendAsync(request);
            await CheckResponse(response);
        }
    }
}
