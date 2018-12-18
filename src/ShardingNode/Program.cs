using System;
using Akka.Actor;
using Akka.Cluster.Sharding;

namespace ShardingNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var system = ActorSystem.Create("akka-lab");
            var region = ClusterSharding.Get(system).StartAsync(
                typeName: "sharding-actor",
                entityProps: Props.Create<MyActor>(),
                settings: ClusterShardingSettings.Create(system),
                messageExtractor: new MessageExtractor()).Result;

            // send message to entity through shard region
            region.Tell(new ShardEnvelope(shardId: 1, entityId: 1, message: "hello"));
        }
    }
    public class MyActor : ReceiveActor
    {

    }
    public sealed class ShardEnvelope
    {
        public readonly int ShardId;
        public readonly int EntityId;
        public readonly object Message;
        public ShardEnvelope(int shardId, int entityId, object message)
        {

            ShardId = shardId;
            EntityId = entityId;
            Message = message;
        }
    }

    // define, how shard id, entity id and message itself should be resolved
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as ShardEnvelope)?.EntityId.ToString();

        public string ShardId(object message) => (message as ShardEnvelope)?.ShardId.ToString();

        public object EntityMessage(object message) => (message as ShardEnvelope)?.Message;
    }
}
