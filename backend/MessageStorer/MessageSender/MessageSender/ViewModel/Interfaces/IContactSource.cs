using MessageSender.Model;
using System.Collections.Generic;

namespace MessageSender.ViewModel.Interfaces
{
    public interface IContactSource
    {
        IEnumerable<Contact> GetAll();
    }
}
