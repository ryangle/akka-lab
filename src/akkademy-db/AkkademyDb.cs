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
            });
            ReceiveAny(msg =>
            {
                _logger.Info($"received unknown message: {msg}");
            });
        }
    }
}
