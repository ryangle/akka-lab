using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;

namespace akkademy_db
{
    public class AkkademyDb : ReceiveActor
    {
        public readonly Dictionary<string, object> Map = new Dictionary<string, object>();
        protected ILoggingAdapter _logger = Context.GetLogger();

        public AkkademyDb()
        {
            Receive<SetRequest>(msg =>
            {
                _logger.Info($"Received Set request:{msg}");
                Map[msg.Key] = msg.Value;
                Sender.Tell(new Status.Success(msg.Key));
            });
            Receive<GetRequest>(msg =>
            {
                _logger.Info($"Received Get request: {msg}");
                if (Map.ContainsKey(msg.Key))
                {
                    Sender.Tell(Map[msg.Key]);
                }
                else
                {
                    Sender.Tell(new Status.Failure(new KeyNotFoundException(msg.Key)));
                }
            });
            ReceiveAny(msg =>
            {
                _logger.Info($"received unknown message: {msg}");
                Sender.Tell(new Status.Failure(new Exception("unknown message")));
            });
        }
    }
}
