using System;
using System.IO;
using System.Net;
using Java.Lang;
using MobileRouterManagement.Core.Model;
using Renci.SshNet;

namespace MobileRouterManagement.Core.Connection
{
    public static class SshConnection
    {
        #region properties

        private static SshClient sshclient { get; set; }
        private static ShellStream stream { get; set; }
        private static StreamWriter writer { get; set; }
        private static StreamReader reader { get; set; }

        #endregion

        #region Connect methods

        public static void Connect(IPAddress adresIp, int port, string username, string password)
        {
            sshclient = new SshClient(adresIp.ToString(), port, username, password);
            initializeConnection();
        }

        public static void Connect(IPAddress adresIp, string username, string password)
        {
            sshclient = new SshClient(adresIp.ToString(), username, password);
            initializeConnection();
        }

        public static void Connect(string adresIp, string username, string password)
        {
            sshclient = new SshClient(adresIp, username, password);
            initializeConnection();
        }

        public static void Connect(string adresIp, int port, string username, string password)
        {
            sshclient = new SshClient(adresIp, port, username, password);
            initializeConnection();
        }

        public static void Connect(RouterAccesData routerAccesData)
        {
            if (routerAccesData.Port == null || routerAccesData.Port == 0)
            {
                sshclient = new SshClient(routerAccesData.RouterIp.ToString(),
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
            else
            {
                sshclient = new SshClient(routerAccesData.RouterIp.ToString(),
                    Convert.ToInt32(routerAccesData.Port),
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
            initializeConnection();
        }

        #endregion

        public static bool IsConnected()
        {
            return sshclient.IsConnected;
        }

        public static string SendCommand(string customCmd)
        {
            var strAnswer = new StringBuilder();

            writeStream(customCmd);

            strAnswer.Append(readStream());

            var answer = strAnswer.ToString().Trim();

            var unrecognizedCommandAnswer = $"-ash: {customCmd}: not found";
            if (answer.Contains(unrecognizedCommandAnswer))
            {
                throw new InvalidOperationException($"Unrecognized command: {customCmd}");
            }

            var entryWithTypedNameNotExist = "Entry not found";
            if (answer.Contains(entryWithTypedNameNotExist))
            {
                throw new InvalidOperationException($"Entry not found for command: {customCmd}");
            }

            return answer;
        }

        #region private methods

        private static void initializeConnection()
        {
            sshclient.Connect();
            stream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            SendCommand("reset");
        }

        private static void writeStream(string cmd)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        private static string readStream()
        {
            var result = new StringBuilder();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result.Append(line);
                result.Append('\n');
            }

            return result.ToString();
        }

        #endregion
    }
}