using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender.Model.Http
{
    public class UserHttpSender : HttpSender
    {
        const string URL = "/api/AppUsers/login";
        public UserHttpSender(string baseAddress) : base(baseAddress)
        {
        }

        public async Task Login(LoginDetails loginDetails)
        {
            var json = JsonConvert.SerializeObject(loginDetails);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _http.PostAsync(URL, data);
            await CheckResponse(result);
            var body = await result.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<LoggedUser>(body);
            var token = result.Headers.GetValues("Authorization").FirstOrDefault();
            var session = new SessionStorage();
            await session.StoreSession(token, user);
        }
    }
}
