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
            IActorRef replicator = DistributedData.Get(Context.System).Replicator;
            Receive<string>(msg =>
            {
                
                //var keyb = new ORDictionaryKey<string, LWWRegister<string>>("keyB");

                var response = replicator.Ask<IGetResponse>(Dsl.Get(key, Dsl.ReadLocal)).Result;
                if (response.IsSuccessful)
                {
                    var data = response.Get(key);
                    //Console.WriteLine($"read {data.Count}");
                    var es = data.Elements;
                    foreach (var item in es)
                    {
                        Console.WriteLine($"read {es.Count} data {item.Name} at {DateTime.Now}");
                    }
                }
                else
                {
                    Console.WriteLine($"read {response.ToString()} at {DateTime.Now}");
                }
            });
        }
        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, "ok", Self);
        }
    }
}
