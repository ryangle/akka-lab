using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace Common
{
    public class RemoteRefActor : ReceiveActor
    {
        public class MsgWhithRef
        {
            public MsgWhithRef(IActorRef actor)
            {
                Actor = actor;
            }
            public IActorRef Actor { get; }
        }
        public class Create { }

        public RemoteRefActor()
        {
            Receive<string>(msg =>
            {
                Console.WriteLine($"RemoteRefActor receive {msg} form {Sender.Path}");
            });
        }
    }
}
