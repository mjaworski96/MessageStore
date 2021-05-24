using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MessageSender.Model;

namespace MessageSender.ViewModel
{
    public class BaseViewModel
    {
        public string ServerIp
        {
            get
            {
                return "http://192.168.1.122:5000";
            }
        }
    }
}
