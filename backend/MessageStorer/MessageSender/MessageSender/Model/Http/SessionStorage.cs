using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MessageSender.Model.Http
{
    public class SessionStorage
    {
        public async Task StoreSession(string token, LoggedUser loggedUser)
        {
            await SecureStorage.SetAsync("token", token);
            await SecureStorage.SetAsync("user", JsonConvert.SerializeObject(loggedUser));
        }
        public void Clear()
        {
            SecureStorage.RemoveAll();
        }
        public async Task<string> GetToken()
        {
            return await SecureStorage.GetAsync("token");
        }
        public async Task<LoggedUser> GetUser()
        {
            var json = await SecureStorage.GetAsync("user");
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<LoggedUser>(json);
        }
        public bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(GetToken().Result);
        }
    }
}
