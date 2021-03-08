using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Common;

namespace Terminal
{
    class Program
    {
        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);
        static ActorSystem ActorSystem;
        static void Main(string[] args)
        {
            Console.Title = "Terminal";

            var port = "";
            if (args != null && args.Length > 0)
            {
                port = args[0];
            }
            //var actorSystem = ActorSystemFactory.Create(port);
            //var cluster = Cluster.Get(actorSystem);
            //cluster.RegisterOnMemberUp(() =>
            //{
            //    Console.WriteLine($"RegisterOnMemberUp");
            //    actorSystem.ActorOf<ClusterMoniter>();
            //    var terminal = actorSystem.ActorOf<TerminalActor>("terminal");
            //    var input = string.Empty;
            //    while (input != "quit")
            //    {
            //        input = Console.ReadLine();
            //        if (input != null && input.StartsWith("ref"))
            //        {
            //            terminal.Tell(new RemoteRefActor.Create());
            //            Console.WriteLine($"send RemoteRefActor.Create");
            //            continue;
            //        }
            //        if (input != null && input.StartsWith("ask"))
            //        {
            //            var result = terminal.Ask<string>(new RemoteRefActor.GetRandomNum()).Result;
            //            Console.WriteLine($"ask result:{result}");
            //            continue;
            //        }
            //        terminal.Tell(new BinaryFunctionActor.Arguments
            //        {
            //            LeftArg = 10,
            //            RightArg = 20,
            //            Function = "add"
            //        });
            //        Console.WriteLine($"send");
            //    }

            //});
            CreatSystem();
            //_manualResetEventSlim.Wait();
            Console.ReadLine();

            // var cluster = Cluster.Get(actorSystem);
            //cluster.RegisterOnMemberRemoved(() => MemberRemoved(actorSystem));
            //cluster.Leave(cluster.SelfAddress);
        }
        private static void CreatSystem()
        {
            Console.WriteLine($"CreatSystem");
            ActorSystem = ActorSystemFactory.Create("");
            ActorSystem.ActorOf<TerminalActor>("terminal");
            var cluster = Cluster.Get(ActorSystem);
            //cluster.RegisterOnMemberUp(MemberUp);
            cluster.RegisterOnMemberRemoved(() => MemberRemoved());
        }
        private static void MemberUp()
        {
            Console.WriteLine($"MemberUp");
            ActorSystem.ActorOf<ClusterMoniter>();
            var terminal = ActorSystem.ActorOf<TerminalActor>("terminal");
            var input = string.Empty;
            while (input != "quit")
            {
                input = Console.ReadLine();
                if (input != null && input.StartsWith("ref"))
                {
                    terminal.Tell(new RemoteRefActor.Create());
                    Console.WriteLine($"send RemoteRefActor.Create");
                    continue;
                }
                if (input != null && input.StartsWith("ask"))
                {
                    var result = terminal.Ask<string>(new RemoteRefActor.GetRandomNum()).Result;
                    Console.WriteLine($"ask result:{result}");
                    continue;
                }
                terminal.Tell(new BinaryFunctionActor.Arguments
                {
                    LeftArg = 10,
                    RightArg = 20,
                    Function = "add"
                });
                Console.WriteLine($"send");
            }
        }
        private static async void MemberRemoved()
        {
            Console.WriteLine($"MemberRemoved");
            Cluster.Get(ActorSystem).Down(Cluster.Get(ActorSystem).SelfAddress);
            //await actorSystem.Terminate();
            await Task.Delay(1000 * 3);
            CreatSystem();
            await Task.CompletedTask;
        }
    }
}