using Akka.Actor;
using System;

namespace FaultToleranceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var system = ActorSystem.Create("system"))
            {
                var supervisor = system.ActorOf<Supervisor>();

                supervisor.Tell("exception");

                Console.ReadLine();
                supervisor.Tell("xxx");
                Console.ReadLine();
            }
        }
    }
}
