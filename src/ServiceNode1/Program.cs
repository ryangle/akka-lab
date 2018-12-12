using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;

namespace SeedNode1
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("akka-cluster", "seednode1");
        private static readonly ManualResetEvent quitEvent = new ManualResetEvent(false);
        private static readonly ManualResetEvent asTerminatedEvent = new ManualResetEvent(false);

        /// <summary>
        /// SEED NODE PROGRAM
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                quitEvent.Set();
                e.Cancel = true;
            };

            log.Debug("Creating actor system");
            var config = Akka.Configuration.ConfigurationFactory.ParseString(@"
                akka {
                    actor {
                        provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                        serializers {
                            wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
                        }
                        serialization - bindings {
                            ""System.Object"" = wire
                        }
                    }
                    remote {
                        helios.tcp {
                            transport -class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
						    applied-adapters = []
                            transport-protocol = tcp
                            hostname = ""127.0.0.1""
                            port = 0
					    }
                    }
                    loggers = [""Akka.Logger.log4net.Log4NetLogger,Akka.Logger.log4net""]
                    cluster {
					    seed-nodes = [""akka.tcp://sample@127.0.0.1:7001""]
                        roles = [seednode1]
				    }
			    }
            ");
            ActorSystem actorSystem = ActorSystem.Create("sample", config);

            quitEvent.WaitOne();

            log.Info("Shutting down");

            var cluster = Cluster.Get(actorSystem);
            cluster.RegisterOnMemberRemoved(() => MemberRemoved(actorSystem));
            cluster.Leave(cluster.SelfAddress);

            asTerminatedEvent.WaitOne();
            log.Info("Actor system terminated, exiting");
            if (System.Diagnostics.Debugger.IsAttached) Console.ReadLine();
        }

        private static async void MemberRemoved(ActorSystem actorSystem)
        {
            await actorSystem.Terminate();
            asTerminatedEvent.Set();
        }
    }
}
