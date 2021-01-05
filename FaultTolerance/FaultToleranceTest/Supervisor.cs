using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultToleranceTest
{
    class Supervisor : ReceiveActor
    {
        public Supervisor()
        {
            Console.WriteLine("Create Supervisor");
            Receive<Props>(msg =>
            {
                var child = Context.ActorOf(msg); // create child
                Sender.Tell(child); // send back reference to child actor
            });
            Receive<string>(msg =>
            {
                Console.WriteLine($"Receive {msg}");
                if (msg == "exception")
                {
                    throw new Exception("super exception");
                }
            });
        }
        public override void AroundPostRestart(Exception cause, object message)
        {
            Console.WriteLine("AroundPostRestart");
            base.AroundPostRestart(cause, message);
        }
        public override void AroundPostStop()
        {
            Console.WriteLine("AroundPostStop");
            base.AroundPostStop();
        }
        public override void AroundPreRestart(Exception cause, object message)
        {
            Console.WriteLine("AroundPreRestart");
            base.AroundPreRestart(cause, message);
        }
        public override void AroundPreStart()
        {
            Console.WriteLine("AroundPreStart");
            base.AroundPreStart();
        }
        protected override bool AroundReceive(Receive receive, object message)
        {
            Console.WriteLine("AroundReceive");
            return base.AroundReceive(receive, message);
        }
        protected override void PostRestart(Exception reason)
        {
            Console.WriteLine("PostRestart");
            base.PostRestart(reason);
        }
        protected override void PostStop()
        {
            Console.WriteLine("PostStop");
            base.PostStop();
        }
        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine("PreRestart");
            base.PreRestart(reason, message);
        }
        protected override void PreStart()
        {
            Console.WriteLine("PreStart");
            base.PreStart();
        }
        //protected override SupervisorStrategy SupervisorStrategy()
        //{
        //    return new OneForOneStrategy(
        //        maxNrOfRetries: 10,
        //        withinTimeRange: TimeSpan.FromMinutes(1),
        //        localOnlyDecider: ex =>
        //        {
        //            switch (ex)
        //            {
        //                case ArithmeticException ae:
        //                    return Directive.Resume;
        //                case NullReferenceException nre:
        //                    return Directive.Restart;
        //                case ArgumentException are:
        //                    return Directive.Stop;
        //                default:
        //                    return Directive.Escalate;
        //            }
        //        });
        //}
    }
}
