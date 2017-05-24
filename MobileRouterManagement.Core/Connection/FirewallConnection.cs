using MobileRouterManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MobileRouterManagement.Core.Models.ViewModels;

namespace MobileRouterManagement.Core.Connection
{
    public static class FirewallConnection
    {
        public static IEnumerable<FirewallRule> Get_AllFirewallRestrictionRules()
        {
            var rulesNames = Get_RestrictionRulesNames();

            return rulesNames.Select(Get_FirewallRuleByName);
        }

        public static FirewallRule Get_FirewallRuleByName(string ruleName)
        {
            if (string.IsNullOrEmpty(ruleName))
            {
                throw new Exception("Can't get rule without name!");
            }

            if (ruleName.Any(char.IsWhiteSpace))
            {
                throw new Exception("Rule name contains whitespaces!");
            }

            var routerConfig = SshConnection.Send_CustomCommand($"uci show firewall.{ruleName}");
            var currentConfiguratrion = SshConnection.ParseAnswerToDictionary(routerConfig);

            var rule = new FirewallRule { RuleName = ruleName };

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.name"))
            {
                rule.FriendlyName = currentConfiguratrion[$"firewall.{ruleName}.name"].Trim('\'');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_mac"))
            {
                rule.Src_mac = currentConfiguratrion[$"firewall.{ruleName}.src_mac"].Trim().Trim('\'').Split(',');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_ip"))
            {
                rule.Src_ip = currentConfiguratrion[$"firewall.{ruleName}.src_ip"].Trim().Trim('\'').Split(',');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_port"))
            {
                rule.Src_port = currentConfiguratrion[$"firewall.{ruleName}.src_port"].Trim().Trim('\'').Split(',');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.dest_ip"))
            {
                rule.Dest_ip = currentConfiguratrion[$"firewall.{ruleName}.dest_ip"].Trim().Trim('\'').Split(',');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.dest_port"))
            {
                rule.Dest_port = currentConfiguratrion[$"firewall.{ruleName}.dest_port"].Trim().Trim('\'').Split(',');
            }

            if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.enabled"))
            {
                rule.Enabled = currentConfiguratrion[$"firewall.{ruleName}.enabled"].Trim('\'');
            }

            return rule;
        }

        public static void Send_DeleteFirewallRule(string ruleName)
        {
            SshConnection.WriteStream($"uci delete firewall.{ruleName}");
            SshConnection.WriteStream($"uci commit firewall");
            SshConnection.Send_CustomCommand($"/etc/init.d/firewall restart");

            Thread.Sleep(1500);
        }

        public static string Send_SaveFirewallRule(AddFirewallRuleViewModel rule)
        {
            var ruleName = saveFirewallRule(rule);

            SshConnection.WriteStream($"uci commit firewall");
            SshConnection.Send_CustomCommand($"/etc/init.d/firewall restart");

            Thread.Sleep(1500);
            SshConnection.Send_CustomCommand($"clear");

            return ruleName;
        }

        public static IEnumerable<string> Get_RestrictionRulesNames()
        {
            var answer = SshConnection.Send_CustomCommand("grep RouterManagementRule /etc/config/firewall");

            return from Match match in Regex.Matches(answer, "(RouterManagementRule_)([0-9]+)")
                   select match.ToString();
        }

        private static string saveFirewallRule(AddFirewallRuleViewModel rule)
        {
            var ruleName = (string.IsNullOrEmpty(rule.RuleName)) ? getNewRestrictionRuleName() : rule.RuleName;

            SshConnection.WriteStream($"uci set firewall.{ruleName}=rule");
            SshConnection.WriteStream($"uci set firewall.{ruleName}.src='*'");
            SshConnection.WriteStream($"uci set firewall.{ruleName}.dest='*'");
            SshConnection.WriteStream($"uci set firewall.{ruleName}.name='{rule.FriendlyName}'");
            if (!string.IsNullOrEmpty(rule.SourceMacs)) SshConnection.WriteStream($"uci set firewall.{ruleName}.src_mac='{rule.SourceMacs}'");
            if (!string.IsNullOrEmpty(rule.SourceIPs)) SshConnection.WriteStream($"uci set firewall.{ruleName}.src_ip='{rule.SourceIPs}'");
            if (!string.IsNullOrEmpty(rule.SourcePorts)) SshConnection.WriteStream($"uci set firewall.{ruleName}.src_port='{rule.SourcePorts}'");
            if (!string.IsNullOrEmpty(rule.DestinationIPs)) SshConnection.WriteStream($"uci set firewall.{ruleName}.dest_ip='{rule.DestinationIPs}'");
            if (!string.IsNullOrEmpty(rule.DestinationPorts)) SshConnection.WriteStream($"uci set firewall.{ruleName}.dest_port='{rule.DestinationPorts}'");
            SshConnection.WriteStream($"uci set firewall.{ruleName}.target='DROP'");
            SshConnection.WriteStream($"uci set firewall.{ruleName}.enabled='{rule.Enabled}'");

            return ruleName;
        }

        private static string getNewRestrictionRuleName()
        {
            var howMuchToDelete = "RouterManagementRule_".Length;
            var rulesNumbers = Get_RestrictionRulesNames().Select(r => Convert.ToInt32(r.Remove(0, howMuchToDelete))).OrderBy(r => r).ToList();

            if (!rulesNumbers.Any())
            {
                return "RouterManagementRule_1";
            }

            var numbersBetween = Enumerable.Range(1, rulesNumbers.Last()).Except(rulesNumbers).ToList();
            return numbersBetween.Any() ? $"RouterManagementRule_{numbersBetween.First()}" : $"RouterManagementRule_{rulesNumbers.Last() + 1}";
        }
    }
}