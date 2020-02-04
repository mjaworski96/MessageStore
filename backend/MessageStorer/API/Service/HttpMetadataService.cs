using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IHttpMetadataService
    {
        string Username { get; }
        string Application { get; }
    }
    public class MockHttpMetadataService : IHttpMetadataService
    {
        public string Username => "test";

        public string Application => "sms";
    }
}
