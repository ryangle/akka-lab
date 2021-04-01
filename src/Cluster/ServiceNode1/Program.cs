using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Common;

namespace SeedNode1
{
    class Program
    {
        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            Console.Title = "ServiceNode";
            Console.WriteLine("Service Node 1 Start");
            var port = "";
            if (args != null && args.Length > 0)
            {
                port = args[0];
            }
            var actorSystem = ActorSystemFactory.Create(port);

            actorSystem.ActorOf<ClusterMoniter>();
            Console.WriteLine("Service Node 1 wait");
            var cluster = Cluster.Get(actorSystem);
            cluster.RegisterOnMemberRemoved(() => MemberRemoved(actorSystem));
            //_manualResetEventSlim.Wait();
            //Console.ReadLine();
            Console.WriteLine("Start ServiceActor");
            var serviceactor = actorSystem.ActorOf<ServiceActor>("serviceEntry");
            //Console.ReadLine();
            //serviceactor.Tell(new GenericMsg<string> { Data="dddd"});
            //Console.ReadLine();
            //serviceactor.Tell(new GenericMsg<int> { Data = 12 });
            Console.ReadLine();
            Console.WriteLine("actorSystem.Terminate()");
            actorSystem.Terminate();
            Console.ReadLine();
            Console.WriteLine("Service Node 1 leave");
            //var cluster = Cluster.Get(actorSystem);
            //cluster.RegisterOnMemberRemoved(() => MemberRemoved(actorSystem));
            //cluster.Leave(cluster.SelfAddress);
        }

        private static async void MemberRemoved(ActorSystem actorSystem)
        {
            Console.WriteLine("MemberRemoved.....");
            await actorSystem.Terminate();
        }
    }
}
