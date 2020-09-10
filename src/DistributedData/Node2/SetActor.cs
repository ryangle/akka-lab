using Akka.Actor;
using Akka.Cluster;
using Akka.DistributedData;
using DataCommon;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Node2
{
    class SetActor : ReceiveActor
    {
        public int count;
        public SetActor()
        {
            Receive<string>(msg =>
            {
                var s = count;
                var cluster = Cluster.Get(Context.System);
                var replicator = DistributedData.Get(Context.System).Replicator;

                var key = new ORSetKey<User>("keyA");
                var writeConsistency = new WriteTo(3, TimeSpan.FromSeconds(1));

                replicator.Tell(Dsl.Update(key, new ORSet<User>().Add(cluster.SelfUniqueAddress, new User { Name = s.ToString() }), writeConsistency, old =>
                {
                    if (s >= 3)
                    {
                        return old
                        .Add(cluster.SelfUniqueAddress, new User { Name = s.ToString() })
                        .Remove(cluster.SelfUniqueAddress, new User { Name = (s - 3).ToString() });
                    }
                    else
                    {
                        return old.Add(cluster.SelfUniqueAddress, new User { Name = s.ToString() });
                    }
                }));

                count++;
            });
        }
        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), Self, count.ToString(), Self);
            //Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, "ok", Self);
        }
    }
}
