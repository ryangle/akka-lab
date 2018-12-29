using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using akkademy_db;

namespace AkkademydbClient
{
    public class NClient
    {
        private readonly ActorSystem system = ActorSystem.Create("localsystem");
        private readonly ActorSelection remoteDb;
        public NClient(string remoteAddress)
        {
            remoteDb = system.ActorSelection($"akka.tcp://akkademy@{remoteAddress}/user/akkademy-db");
        }

        public Task Set(string key, object value)
        {
            return remoteDb.Ask(new SetRequest(key, value), TimeSpan.FromSeconds(2));
        }

        public Task<object> Get(string key)
        {
            return remoteDb.Ask<object>(new GetRequest(key), TimeSpan.FromSeconds(2));
        }
    }
}
