using System;
using System.IO;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using akkademy_db;

namespace AkkademydbServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorConfig = ConfigurationFactory.ParseString(
                File.ReadAllText("akka.conf"));
            var system = ActorSystem.Create("akkademy", actorConfig);
            system.ActorOf<AkkademyDb>("akkademy-db");

            Console.ReadLine();
        }
    }
}
