using MobileRouterManagement.Core.Model;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MobileRouterManagement.Core.Connection
{
    public static class SshConnection
    {
        #region properties

        public static SshClient sshclient { get; private set; }
        public static bool IsConnected => sshclient != null && sshclient.IsConnected;

        private static ShellStream shellStream;

        #endregion properties

        public static void Connect(string adresIp, string username, string password)
        {
            sshclient = new SshClient(adresIp, username, password);

            sshclient.Connect();

            shellStream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);

            Send_CustomCommand("reset");
        }

        public static string Send_CustomCommand(string customCmd)
        {
            var reader = new StreamReader(shellStream);
            reader.ReadToEnd(); //clear stream from old data
            WriteStream(customCmd);

            var strAnswer = new StringBuilder();
            strAnswer.AppendLine(reader.ReadToEnd());
            var answer = strAnswer.ToString().Trim().Replace("\'", string.Empty);

            if (answer.Contains(string.Concat("-ash: ", customCmd, ": not found")))
            {
                throw new InvalidOperationException(string.Concat("Unrecognized command: ", customCmd));
            }

            if (answer.Contains("Entry not found"))
            {
                throw new InvalidOperationException(string.Concat("Entry not found for command: ", customCmd));
            }

            return answer;
        }

        public static Wireless Get_Wireless()
        {
            var answer = Send_CustomCommand("uci show wireless");

            var wirelessConfiguratrion = ParseAnswerToDictionary(answer);

            var wireless = new Wireless
            {
                Disabled = (wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".disabled")).Value == "1"),
                Channel = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".channel")).Value,
                Ssid = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".ssid")).Value,
                Encryption = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".encryption")).Value,
                Key = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".key")).Value,
                Mode = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".mode")).Value,
                Network = wirelessConfiguratrion.FirstOrDefault(c => c.Key.Contains(".network")).Value
            };

            return wireless;
        }

        public static void Send_SaveWireless(Wireless wireless)
        {
            WriteStream($"uci set wireless.@wifi-device[0].disabled={Convert.ToInt32(wireless.Disabled)}");
            WriteStream($"uci set wireless.@wifi-device[0].channel={wireless.Channel}");
            WriteStream($"uci set wireless.@wifi-iface[0].ssid={wireless.Ssid}");
            WriteStream($"uci set wireless.@wifi-iface[0].encryption={wireless.Encryption}");
            WriteStream($"uci set wireless.@wifi-iface[0].key={wireless.Key}");
            WriteStream($"uci set wireless.@wifi-iface[0].mode={wireless.Mode}");
            WriteStream($"uci set wireless.@wifi-iface[0].network={wireless.Network}");
            if (wireless.Network == "wan")
            {
                WriteStream($"uci set network.wan=interface");
                WriteStream($"uci set network.wan.proto=dhcp");
            }

            WriteStream($"uci commit");
            Send_CustomCommand($"wifi");
            Thread.Sleep(5000);
        }

        public static void Disconnect()
        {
            sshclient.Disconnect();
        }

        #region private methods

        public static Dictionary<string, string> ParseAnswerToDictionary(string answer)
        {
            //remove first line - command sent to router
            answer = answer.Substring(answer.IndexOf(Environment.NewLine, StringComparison.Ordinal) + 1);
            //remove two last lines (unrecognized log information)
            answer = answer.Remove(answer.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
            //remove new line mark as first char
            if (answer.FirstOrDefault().Equals('\n'))
            {
                answer = answer.Remove(0, 1);
            }

            var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var entriesAsDictionary = entriesTable
                .Select(part => part.Split('='))
                .ToDictionary(split => split[0], split => split[1]);

            return entriesAsDictionary;
        }

        public static void WriteStream(string cmd)
        {
            var writer = new StreamWriter(shellStream) { AutoFlush = true };
            writer.WriteLine(cmd);
            while (shellStream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        #endregion private methods
    }
}