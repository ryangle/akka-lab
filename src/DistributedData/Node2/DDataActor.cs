using Akka.Actor;
using Akka.DistributedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node2
{
    public class DDataActor : ReceiveActor
    {
        private IActorRef _replicator = DistributedData.Get(Context.System).Replicator;
        public DDataActor()
        {
            Receive<Get>(msg =>
            {
                Console.WriteLine("DDateAcor Get");
                var response = _replicator.Ask<IGetResponse>(msg).Result;
                Sender.Tell(response);
            });
            Receive<Update>(msg =>
            {
                Console.WriteLine("DDateAcor update");
                var response = _replicator.Ask<IUpdateResponse>(msg).Result;
                Sender.Tell(response);
            });
        }
        protected override void PreStart()
        {
        }
    }
}
