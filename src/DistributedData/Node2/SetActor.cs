using Akka.Actor;
using Akka.Cluster;
using Akka.DistributedData;
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

                var key = new ORSetKey<string>("keyA");
                //var set = ORSet.Create(cluster.SelfUniqueAddress, s.ToString());
                var writeConsistency = new WriteTo(3, TimeSpan.FromSeconds(1));
                //var dkey = new ORDictionaryKey<string, LWWRegister<string>>("keyB");
                //var dd = ORDictionary<string, LWWRegister<string>>.Empty.SetItem(cluster, dkey, new LWWRegister<string>(cluster.SelfUniqueAddress, DateTime.Now.ToString()));

                replicator.Tell(Dsl.Update(key, new ORSet<string>().Add(cluster.SelfUniqueAddress,s.ToString()), writeConsistency, old =>
                 {
                     if (s >= 3)
                     {
                         return old
                         .Add(cluster.SelfUniqueAddress, s.ToString())
                         .Remove(cluster.SelfUniqueAddress, (s - 3).ToString());
                     }
                     else
                     {
                         return old.Add(cluster.SelfUniqueAddress, s.ToString());
                     }
                    //if (count % 3 == 0 && count != 0)
                    //{
                    //    Console.WriteLine("remove "+count);
                    //    return old.Remove(cluster.SelfUniqueAddress, (count - 3).ToString());
                    //}
                    //else
                    //{
                    //    Console.WriteLine("add "+count);
                    //    return old.Add(cluster.SelfUniqueAddress, count.ToString());
                    //}
                }
                ));

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
