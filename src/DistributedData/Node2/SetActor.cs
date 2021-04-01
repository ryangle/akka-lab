using Akka.Actor;
using Akka.Cluster;
using Akka.DistributedData;
using DataCommon;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Node2
{
    class SetActor : ReceiveActor
    {
        public int count;

        Cluster cluster = Cluster.Get(Context.System);
        public SetActor()
        {
            IActorRef ddataActor = Context.ActorOf<DDataActor>();
            Receive<ORSetTest>(msg =>
            {
                var s = count;
                //var replicator = DistributedData.Get(Context.System).Replicator;

                var key = new ORSetKey<User>("keyA");
                var writeConsistency = new WriteTo(2, TimeSpan.FromSeconds(1));
                var write = new WriteMajority(TimeSpan.FromSeconds(1));
                Console.WriteLine($"ORSetTest s:{s}");
                ddataActor.Tell(Dsl.Update(key, ORSet<User>.Empty, WriteLocal.Instance, old =>
                {
                    if (s == 0)
                    {
                        Console.WriteLine($"0 old count:{old.Count}");
                        return old;
                    }
                    else if (s == 1)
                    {
                        Console.WriteLine($"1 old count:{old.Count}");

                        return old.Add(cluster, new User { Name = s.ToString(), Count = s.ToString() });
                    }
                    else if (s > 1)
                    {
                        var u = old.FirstOrDefault();
                        if (u != null)
                        {
                            u.Count = s.ToString();
                            Console.WriteLine($"1>old name:{u.Name},count:{old.Count}");

                            //return old.Add(cluster, u);
                            //return old.Remove(cluster, u).Add(cluster, u);
                            return old;
                        }


                    }
                    Console.WriteLine($"end old count:{old.Count}");

                    return old;
                    //if (s >= 3)
                    //{
                    //    return old
                    //    .Add(cluster.SelfUniqueAddress, new User { Name = s.ToString() })
                    //    .Remove(cluster.SelfUniqueAddress, new User { Name = (s - 3).ToString() });
                    //}
                    //else
                    //{
                    //    return old.Add(cluster.SelfUniqueAddress, new User { Name = s.ToString() });
                    //}
                }));

                count++;
            });

            Receive<LWWDictionaryTest>(msg =>
            {
                try
                {
                    var cluster = Cluster.Get(Context.System);
                    var replicator = DistributedData.Get(Context.System).Replicator;

                    var key = new LWWDictionaryKey<string, User>("keyB");

                    var writeConsistency = new WriteTo(3, TimeSpan.FromSeconds(1));

                    var initial = LWWDictionary<string, User>.Empty;
                    replicator.Tell(Dsl.Update(key, initial, writeConsistency, old =>
                    {
                        //var m = old.Merge(initial);
                        var m = old.SetItem(cluster.SelfUniqueAddress, count.ToString(), new User { Name = count.ToString() });
                        if (count > 3)
                        {
                            m = m.Remove(cluster.SelfUniqueAddress, (count - 1).ToString());
                            m = old.SetItem(cluster.SelfUniqueAddress, "3", new User { Name = count.ToString() });

                        }
                        return m;
                        //return old;
                    }));
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LWWDictionaryData error:{ex.Message}");
                }

            });

            Receive<ORDictionaryTest>(msg =>
            {
                try
                {
                    //var cluster = Cluster.Get(Context.System);
                    //var replicator = DistributedData.Get(Context.System).Replicator;

                    //var key = new ORDictionaryKey<string,ORSet<User>>("keyC");

                    //var writeConsistency = new WriteTo(3, TimeSpan.FromSeconds(1));

                    //var set =ORSet.Empty.
                    //var initial = ORDictionary.Create(cluster.SelfUniqueAddress, count.ToString(), new User { Name = count.ToString() });
                    //replicator.Tell(Dsl.Update(key, initial, writeConsistency, old =>
                    //{
                    //    return initial;
                    //    //return old;
                    //}));
                    //count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LWWDictionaryData error:{ex.Message}");
                }

            });

            Receive<LWWRegisterTest>(msg =>
            {
                try
                {
                    var cluster = Cluster.Get(Context.System);
                    var replicator = DistributedData.Get(Context.System).Replicator;

                    var key = new LWWRegisterKey<User>("keyLWWResigter");

                    var writeConsistency = new WriteTo(3, TimeSpan.FromSeconds(1));

                    var initial = new LWWRegister<User>(cluster.SelfUniqueAddress, new User());
                    replicator.Tell(Dsl.Update(key, initial, writeConsistency, old =>
                    {
                        return new LWWRegister<User>(cluster.SelfUniqueAddress, new User { Name = count.ToString() });
                    }));
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LWWRegisterTest error:{ex.Message}");
                }
            });
        }
        protected override void PreStart()
        {
            //Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), Self, new LWWDictionaryTest(), Self);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), Self, new ORSetTest(), Self);
            //Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), Self, new LWWRegisterTest(), Self);
        }
    }
}
