using System;
using System.IO;
using System.Threading;
using Common;
using Akka.Actor;
using Akka.Cluster;

namespace Lighthouse
{
    class Program
    {
        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            Console.Title = "Lighthouse";
            var port = "";
            if (args != null && args.Length > 0)
            {
                port = args[0];
            }
            var actorSystem = ActorSystemFactory.Create(port);

            

            Cluster.Get(actorSystem).RegisterOnMemberUp(() =>
            {
                Console.WriteLine($"RegisterOnMemberUp,{Cluster.Get(actorSystem).SelfAddress}");
                actorSystem.ActorOf<ClusterMoniter>();
            });

            //var deadletterWatchMonitorProps = Props.Create(() => new ActorSystemMonitor());
            //var deadletterWatchActorRef = actorsystem.ActorOf(deadletterWatchMonitorProps);
            //actorsystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
            actorSystem.WhenTerminated.Wait();
            //_manualResetEventSlim.Wait();
        }
    }
}