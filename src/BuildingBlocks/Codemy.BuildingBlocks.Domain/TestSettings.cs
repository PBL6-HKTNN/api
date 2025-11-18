using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.BuildingBlocks.Domain
{
    public class TestSettings
    {
        public ApiGatewaySettings ApiGateway { get; set; }
        public TestUserSettings TestUser { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
    }

    public class ApiGatewaySettings
    {
        public string BaseUrl { get; set; }
        public string GatewayPath { get; set; }
        public int Timeout { get; set; }
        public bool IgnoreSslErrors { get; set; }
    }

    public class TestUserSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
