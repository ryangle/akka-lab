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
        private int count = 0;
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
            Receive<string>(msg =>
            {
                if (count % 2 == 1)
                {
                    serviceEntry.Tell(new GenericMsg<string> { Data = count.ToString() });

                }
                else
                {
                    serviceEntry.Tell(new GenericMsg<int> { Data = count });
                }
                count++;
                //serviceEntry.Tell(msg);

                Console.WriteLine($"Receive msg:{msg}");
            });
        }
        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, "ok", Self);
        }
    }
}
