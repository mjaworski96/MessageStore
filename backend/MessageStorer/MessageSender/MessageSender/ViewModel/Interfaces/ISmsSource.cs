using MessageSender.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MessageSender.ViewModel.Interfaces
{
    public interface ISmsSource
    {
        IEnumerable<Sms> GetAll(DateTime from);
        int GetCount(DateTime from);
    }
}
