using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Cluster.Tools.Client;
using Akka.Configuration;

namespace Common
{
    public static class ActorSystemFactory
    {
        public static ActorSystem Create(string port = "")
        {
            var systemConfig = ConfigurationFactory.ParseString(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "akka.conf")));

            var seedNodes = systemConfig.GetConfig("akka.cluster").GetStringList("seed-nodes");
            var systemName = string.Empty;
            if (seedNodes != null && seedNodes.Count > 0)
            {
                var match = Regex.Match(seedNodes[0], "(?<=://)[\\w|-]+");
                if (match.Success)
                {
                    systemName = match.Value;
                }
            }

            var startConfig = ConfigurationFactory.Empty;

            if (!string.IsNullOrEmpty(port))
            {
                if (int.TryParse(port, out _))
                {
                    startConfig = startConfig.WithFallback(ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {port}"));
                }
            }

            var finalConfig = startConfig.WithFallback(systemConfig);

            return ActorSystem.Create(systemName, systemConfig);
        }
    }
}
