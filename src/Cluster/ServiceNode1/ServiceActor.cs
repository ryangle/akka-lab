using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Cluster;
using Common;

namespace SeedNode1
{
    public class ServiceActor : ReceiveActor
    {
        public const string BinaryFunction = "BinaryFunction";
        protected Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        private UniqueAddress UinAddress;
        public ServiceActor()
        {
            Receive<ClusterEvent.MemberUp>(msg =>
            {
                Console.WriteLine("Receive MemberUp");
                if (msg.Member.Roles.Contains("Terminal"))
                {
                    UinAddress = msg.Member.UniqueAddress;
                    var path = $"{UinAddress.Address}/user/terminal";
                    Console.WriteLine($"ServiceActor Tell {path} ,{msg.Member.Address}MemberUp");

                    Context.ActorSelection(path).Tell("Member up");
                }
            });
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
            Receive<GenericMsg<string>>(msg =>
            {
                Console.WriteLine($"receive GenericMsg string {msg.Data}");
            });
            Receive<GenericMsg<int>>(msg =>
            {
                Console.WriteLine($"receive GenericMsg int {msg.Data}");
            });
            Receive<string>(msg =>
            {
                Console.WriteLine($"receive string {msg}");
            });
        }
        protected override void PreStart()
        {
            Console.WriteLine($"PreStart");
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.MemberUp) });
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(4), Self, "ex", Self);

        }
        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine($"PreRestart");
        }
        protected override void PostStop()
        {
            Console.WriteLine($"PostStop");
            //Cluster.Unsubscribe(Self);
        }
    }
}
