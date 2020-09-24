using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;
using Common;

namespace Terminal
{
    public class CommandActor : ReceiveActor, IWith​Unbounded​Stash
    {
        private IActorRef _serviceEntry = ActorRefs.Nobody;
        private ICancelable _cancelable;
        private IActorRef _binaryFunction = ActorRefs.Nobody;

        public IStash Stash { get; set; }

        public CommandActor(IActorRef serviceEntry)
        {
            _serviceEntry = serviceEntry;
            Receive<ServiceDiscovery>(msg =>
            {
                _cancelable.Cancel();
                _binaryFunction = Sender;
                Become(Ready);
            });

            Receive<BinaryFunctionActor.Arguments>(msg =>
            {
                Stash.Stash();
            });
        }

        private void Ready()
        {
            Console.WriteLine("CommandActor is ready");

            Receive<BinaryFunctionActor.Arguments>(msg =>
            {
                Console.WriteLine("Send BinaryFunctionActor.Arguments");
                _binaryFunction.Tell(msg);
            });
            Receive<BinaryFunctionActor.Result>(msg =>
            {
                Console.WriteLine($"Receive {msg.Data}");
                Context.Stop(Self);
            });
            Stash.UnstashAll();
        }

        protected override void PreStart()
        {
            _cancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromSeconds(10), _serviceEntry, new ServiceDiscovery(), Self);
            base.PreStart();
        }
    }
}
