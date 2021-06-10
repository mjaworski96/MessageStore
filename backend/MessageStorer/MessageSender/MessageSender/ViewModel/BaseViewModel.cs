using MessageSender.Model;

namespace MessageSender.ViewModel
{
    public class BaseViewModel
    {
        public string ServerIp
        {
            get
            {
                return Constraints.URL;
            }
        }
    }
}
