using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http
{
    public class ImportApiClient: ApiClientBase
    {
        public ImportApiClient(SharedToken authorizationToken, HttpClient httpClient) : base(authorizationToken, httpClient)
        {
        }

        public async Task Finish(string importId)
        {
            await Put<string, string>($"/api/messagesImports/{importId}/finish", null);  
        }
    }
}
