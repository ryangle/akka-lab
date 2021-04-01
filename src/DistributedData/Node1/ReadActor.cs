using Akka.Actor;
using Akka.DistributedData;
using DataCommon;
using System;
using System.Collections.Generic;
using System.Text;
//using static Akka.DistributedData.Dsl;

namespace Node1
{
    class ReadActor : ReceiveActor
    {
        public ReadActor()
        {
            ORSetKey<User> key = new ORSetKey<User>("keyA");
            LWWDictionaryKey<string, User> keyB = new LWWDictionaryKey<string, User>("keyB");
            IActorRef replicator = DistributedData.Get(Context.System).Replicator;
            IActorRef ddataActor = Context.ActorOf<DDataActor>();
            Receive<ORSetTest>(msg =>
            {

                //var keyb = new ORDictionaryKey<string, LWWRegister<string>>("keyB");

                var response = ddataActor.Ask<IGetResponse>(Dsl.Get(key, Dsl.ReadLocal)).Result;
                if (response.IsSuccessful)
                {
                    var data = response.Get(key);
                    //Console.WriteLine($"read {data.Count}");
                    var es = data.Elements;
                    var name = string.Empty;
                    var counts = string.Empty;
                    foreach (var item in es)
                    {
                        name += item.Name + ",";
                        counts += item.Count + ",";
                    }
                    Console.WriteLine($"read {es.Count} data name:{name},count:{counts} at {DateTime.Now}");
                }
                else
                {
                    Console.WriteLine($"read {response.ToString()} at {DateTime.Now}");
                }
            });
            Receive<LWWDictionaryTest>(msg =>
            {
                var response = replicator.Ask<IGetResponse>(Dsl.Get(keyB, Dsl.ReadLocal)).Result;
                if (response.IsSuccessful)
                {
                    var data = response.Get(keyB);
                    //Console.WriteLine($"read {data.Count}");
                    var count = 0;
                    foreach (var item in data.Entries)
                    {
                        Console.WriteLine($"IsSuccessful {count++} read {item.Key} data {item.Value.Name} at {DateTime.Now}");
                    }
                }
                else
                {
                    Console.WriteLine($"read {response.ToString()} at {DateTime.Now}");
                }
            });
            Receive<LWWRegisterTest>(msg =>
            {
                try
                {
                    var replicator = DistributedData.Get(Context.System).Replicator;

                    var key = new LWWRegisterKey<User>("keyLWWResigter");

                    var response = replicator.Ask<IGetResponse>(Dsl.Get(key, Dsl.ReadLocal)).Result;
                    if (response.IsSuccessful)
                    {
                        var data = response.Get(key);
                        //Console.WriteLine($"read {data.Count}");

                        Console.WriteLine($"IsSuccessful  data {data.Value.Name} at {DateTime.Now}");
                    }
                    else
                    {
                        Console.WriteLine($"read {response.ToString()} at {DateTime.Now}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LWWRegisterTest error:{ex.Message}");
                }
            });
        }
        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, new ORSetTest(), Self);
            //Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, new LWWRegisterTest(), Self);
            //Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, new LWWDictionaryTest(), Self);
        }
    }
}
