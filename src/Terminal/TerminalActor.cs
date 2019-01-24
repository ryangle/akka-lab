using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;
using Common;

namespace Terminal
{
    public class TerminalActor : ReceiveActor
    {
        public TerminalActor()
        {
            var serviceEntry = Context.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "servicerouter");
            Receive<BinaryFunctionActor.Arguments>(msg =>
            {
                Context.ActorOf(Props.Create(() => new CommandActor(serviceEntry))).Forward(msg);
            });

            Receive<RemoteRefActor.Create>(msg =>
            {
                serviceEntry.Tell(msg);
            });
            Receive<RemoteRefActor.MsgWhithRef>(msg =>
            {
                Console.WriteLine($"TerminalActor receive MsgWhithRef {msg.Actor.Path}");
                msg.Actor.Tell("Terminal Actor tell");
            });
            Receive<RemoteRefActor.GetRandomNum>(msg =>
            {
                serviceEntry.Tell(new RemoteRefActor.RandomNumReq(Sender));
            });
        }
    }
}
