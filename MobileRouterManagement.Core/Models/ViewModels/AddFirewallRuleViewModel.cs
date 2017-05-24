namespace MobileRouterManagement.Core.Models.ViewModels
{
    public class AddFirewallRuleViewModel
    {
        public string RuleName { get; set; }

        public string FriendlyName { get; set; }

        public string SourceMacs { get; set; }

        public string SourceIPs { get; set; }

        public string SourcePorts { get; set; }

        public string DestinationIPs { get; set; }

        public string DestinationPorts { get; set; }

        public string Enabled { get; set; }
    }
}