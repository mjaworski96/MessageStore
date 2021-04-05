using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Infrastructure.Http.Model
{
    public class Attachment
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
