using Akka.Actor;
using Akka.Configuration;
using Akka.DistributedData;
using DataCommon;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Node1
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
                            .WithFallback(ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port=8661"))
                            .WithFallback($"akka.cluster.seed-nodes=[\"akka.tcp://TestCluster@127.0.0.1:8661\"]")
                            .WithFallback(DistributedData.DefaultConfig()); ;
            //.WithFallback(ConfigurationFactory.ParseString("akka.loglevel=DEBUG"))
            //.WithFallback(ConfigurationFactory.ParseString("akka.log-config-on-start=on"))
            //.WithFallback(ConfigurationFactory.ParseString("akka.loggers=[\"Akka.Logger.log4net.Log4NetLogger, Akka.Logger.log4net\"]"));
            System = ActorSystem.Create("TestCluster", config);
            System.ActorOf<ReadActor>();
            //Task.Factory.StartNew(() =>
            //{

            //    try
            //    {
            //        var replicator = DistributedData.Get(System).Replicator;

            //        var key = new LWWRegisterKey<User>("keyLWWResigter");
            //        while (true)
            //        {
            //            var response = replicator.Ask<IGetResponse>(Dsl.Get(key, Dsl.ReadLocal)).Result;
            //            if (response.IsSuccessful)
            //            {
            //                var data = response.Get(key);
            //                //Console.WriteLine($"read {data.Count}");

            //                Console.WriteLine($"IsSuccessful  data {data.Value.Name} at {DateTime.Now}");

            //            }
            //            else
            //            {
            //                Console.WriteLine($"read {response.ToString()} at {DateTime.Now}");
            //            }
            //            Task.Delay(1000).Wait();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"LWWRegisterTest error:{ex.Message}");
            //    }

            //});

            Console.ReadLine();
        }
    }
}
