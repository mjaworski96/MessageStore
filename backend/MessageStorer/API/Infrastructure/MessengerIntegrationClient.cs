using Common.Service;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public interface IMessengerIntegrationClient
    {
        Task DeleteImport(string id);
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
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/import/{id}");
            request.Headers.Add("Authorization", _httpMetadataService.AuthorizationToken);
            await httpClient.SendAsync(request);
        }
    }
}
