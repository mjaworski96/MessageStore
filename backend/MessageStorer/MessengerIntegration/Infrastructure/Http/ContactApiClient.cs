using MessengerIntegration.Infrastructure.Http.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http
{
    public class ContactApiClient : ApiClientBase
    {
        public ContactApiClient(SharedToken authorizationToken, HttpClient httpClient) : base(authorizationToken, httpClient)
        {
        }
        public async Task<ContactWithId> CreateOrUpdateContact(Contact contact)
        {
            return await Put<ContactWithId, Contact>("/api/contacts", contact);
        }
    }
}
