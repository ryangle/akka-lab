using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Common;

namespace Terminal
{
    class Program
    {
        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            Console.Title = "Terminal";

            var port = "";
            if (args != null && args.Length > 0)
            {
                port = args[0];
            }
            var actorSystem = ActorSystemFactory.Create(port);
            var cluster = Cluster.Get(actorSystem);
            cluster.RegisterOnMemberUp(() =>
            {
                var terminal = actorSystem.ActorOf<TerminalActor>("terminal");
                var input = string.Empty;
                while (input != "quit")
                {
                    input = Console.ReadLine();
                    terminal.Tell(new BinaryFunctionActor.Arguments
                    {
                        LeftArg = 10,
                        RightArg = 20,
                        Function = "add"
                    });
                    Console.WriteLine($"send");
                }

            });

            _manualResetEventSlim.Wait();

            // var cluster = Cluster.Get(actorSystem);
            cluster.RegisterOnMemberRemoved(() => MemberRemoved(actorSystem));
            cluster.Leave(cluster.SelfAddress);
        }

        private static async void MemberRemoved(ActorSystem actorSystem)
        {
            await actorSystem.Terminate();
        }
    }
}