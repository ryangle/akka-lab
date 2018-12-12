using System;
using System.IO;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;

namespace Lighthouse
{
    class Program
    {
        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            Console.Title = "Lighthouse";
            #region Akka config

            Config actorConfig = null;
            if (File.Exists("akka.conf"))
            {
                actorConfig = ConfigurationFactory.ParseString(File.ReadAllText("akka.conf"));
            }
            else
            {
                throw new ConfigurationException("not found akka.conf");
            }

            #endregion Akka config

            var actorsystem = ActorSystem.Create("akka-lab", actorConfig);

            //var deadletterWatchMonitorProps = Props.Create(() => new ActorSystemMonitor());
            //var deadletterWatchActorRef = actorsystem.ActorOf(deadletterWatchMonitorProps);
            //actorsystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

            _manualResetEventSlim.Wait();
        }
    }
}
