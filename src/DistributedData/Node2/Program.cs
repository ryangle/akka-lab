using Akka.Actor;
using Akka.Configuration;
using Akka.DistributedData;
using System;

namespace Node2
{
    class Program
    {
        public static ActorSystem System;
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString("akka.extensions=[\"Akka.Cluster.Tools.PublishSubscribe.DistributedPubSubExtensionProvider, Akka.Cluster.Tools\"]")
                            .WithFallback(ConfigurationFactory.ParseString("akka.actor.provider=cluster"))
                            .WithFallback(ConfigurationFactory.ParseString("akka.actor.serializers{hyperion=\"Akka.Serialization.HyperionSerializer,Akka.Serialization.Hyperion\"}"))
                            .WithFallback(ConfigurationFactory.ParseString("akka.actor.serialization-bindings{\"System.Object\"=hyperion}"))
                            .WithFallback(ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.hostname=127.0.0.1"))
                            .WithFallback(ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port=0"))
                            .WithFallback($"akka.cluster.seed-nodes=[\"akka.tcp://TestCluster@127.0.0.1:8661\"]")
                            .WithFallback(DistributedData.DefaultConfig()); ;
            //.WithFallback(ConfigurationFactory.ParseString("akka.loglevel=DEBUG"))
            //.WithFallback(ConfigurationFactory.ParseString("akka.log-config-on-start=on"))
            //.WithFallback(ConfigurationFactory.ParseString("akka.loggers=[\"Akka.Logger.log4net.Log4NetLogger, Akka.Logger.log4net\"]"));
            System = ActorSystem.Create("TestCluster", config);
            System.ActorOf<SetActor>();
            Console.ReadLine();
        }
    }
}
