using System.Collections.Generic;

namespace MobileRouterManagement.Core.Models
{
    public class FirewallRule
    {
        public string RuleName { get; set; }

        public string FriendlyName { get; set; }

        public ICollection<string> Src_mac { get; set; }

        public ICollection<string> Src_ip { get; set; }

        public ICollection<string> Src_port { get; set; }

        public ICollection<string> Dest_ip { get; set; }

        public ICollection<string> Dest_port { get; set; }

        public string Enabled { get; set; }
    }
}