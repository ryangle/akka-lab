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
                Context.ActorOf(Props.Create(()=>new CommandActor(serviceEntry))).Forward(msg);
            });
        }
    }
}
