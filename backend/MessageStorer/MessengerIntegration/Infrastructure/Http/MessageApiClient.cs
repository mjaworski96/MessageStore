using MessengerIntegration.Infrastructure.Http.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http
{
    public class MessageApiClient: ApiClientBase
    {
        public MessageApiClient(SharedToken authorizationToken, HttpClient httpClient) : base(authorizationToken, httpClient)
        {
        }

        public async Task<SyncDateTime> GetSyncTime(int id)
        {
            return await Get<SyncDateTime>($"/api/Messages/syncDateTime?contactId={id}");
        }

        public async Task CreateMessage(Message message)
        {
            await Post("/api/Messages", message);
        }
    }
}
