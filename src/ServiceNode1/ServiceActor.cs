using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Common;

namespace SeedNode1
{
    public class ServiceActor : ReceiveActor
    {
        public const string BinaryFunction = "BinaryFunction";
        public ServiceActor()
        {
            Receive<ServiceDiscovery>(msg =>
            {
                var binaryFunction = Context.Child(BinaryFunction);
                if (binaryFunction.IsNobody())
                {
                    binaryFunction = Context.ActorOf<BinaryFunctionActor>(BinaryFunction);
                }
                binaryFunction.Forward(msg);
            });

            Receive<RemoteRefActor.Create>(msg =>
            {
                var remoteRefActor = Context.ActorOf<RemoteRefActor>();
                Sender.Tell(new RemoteRefActor.MsgWhithRef(remoteRefActor));
            });

            Receive<RemoteRefActor.RandomNumReq>(msg =>
            {
                Console.WriteLine($"Receive RemoteRefActor.RandomNumReq from {msg.Responder}");
                msg.Responder.Tell("1111");
            });
        }
    }
}
