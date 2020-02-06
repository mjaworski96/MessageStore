using MessageSender.Model;
using System.Collections;
using System.Collections.Generic;

namespace MessageSender.ViewModel.Interfaces
{
    public interface ISmsSource
    {
        IEnumerable<Sms> GetAll();
        int GetCount();
    }
}
