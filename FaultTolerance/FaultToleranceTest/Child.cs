using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultToleranceTest
{
    class Child : ReceiveActor
    {
        private int state = 0;
        public Child()
        {
            Receive<Exception>(msg =>
            {
                throw msg;
            });

            Receive<int>(msg =>
            {
                state = msg;
            });

            Receive<string>(msg =>
            {
                if (msg == "get")
                {
                    Sender.Tell(state);
                }
            });

        }
    }
}
