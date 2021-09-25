using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class ImportApiSender : HttpSender
    {
        public ImportApiSender(string baseAddress) : base(baseAddress)
        {
        }

        public async Task Finish(string importId)
        {
            var result = await _http.PutAsync($"/api/messagesImports/{importId}/finish", null);
            await CheckResponse(result);
        }
    }
}
