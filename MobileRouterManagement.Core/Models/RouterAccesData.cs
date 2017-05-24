using System.Net;

namespace MobileRouterManagement.Core.Model
{
    public class RouterAccesData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IPAddress RouterIp { get; set; }
        public int? Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
