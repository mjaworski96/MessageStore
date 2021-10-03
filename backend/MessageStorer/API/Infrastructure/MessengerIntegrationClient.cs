using Common.Service;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public interface IMessengerIntegrationClient
    {
        Task DeleteImport(string id);
        Task DeleteAllImports(int userId);
    }
    public class MessengerIntegrationClient: IMessengerIntegrationClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpMetadataService _httpMetadataService;

        public MessengerIntegrationClient(IHttpClientFactory httpClientFactory, IHttpMetadataService httpMetadataService)
        {
            _httpClientFactory = httpClientFactory;
            _httpMetadataService = httpMetadataService;
        }

        public async Task DeleteImport(string id)
        {
            var httpClient = _httpClientFactory.CreateClient("messengerIntegrationClient");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/MessengerImports/{id}");
            AddAuthorization(request); 
            await httpClient.SendAsync(request);
        }

        public async Task DeleteAllImports(int userId)
        {
            var httpClient = _httpClientFactory.CreateClient("messengerIntegrationClient");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/MessengerImports/user/{userId}");
            AddAuthorization(request); 
            await httpClient.SendAsync(request);
        }

        private void AddAuthorization(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", _httpMetadataService.AuthorizationToken);
        }
    }
}
