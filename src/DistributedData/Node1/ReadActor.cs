using Akka.Actor;
using Akka.DistributedData;
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
            Receive<string>(msg =>
            {
                var replicator = DistributedData.Get(Context.System).Replicator;
                var key = new ORSetKey<string>("keyA");
                var keyb = new ORDictionaryKey<string, LWWRegister<string>>("keyB");
                var readConsistency = Dsl.ReadLocal;

                var response = replicator.Ask<IGetResponse>(Dsl.Get(key, readConsistency)).Result;
                if (response.IsSuccessful)
                {
                    var data = response.Get(key);
                    //Console.WriteLine($"read {data.Count}");
                    var es = data.Elements;
                    foreach (var item in es)
                    {
                        Console.WriteLine($"read {es.Count} data {item} at {DateTime.Now}");
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
