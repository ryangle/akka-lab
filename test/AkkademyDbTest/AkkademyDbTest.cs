using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Xunit;

namespace akkademy_db.Test
{
    public class AkkademyDbTest : TestKit
    {
        [Fact]
        public void ItShouldPlaceKeyValueFromSetMessageIntoMap()
        {
            var actorRef = new TestActorRef<AkkademyDb>(Sys, Props.Create<AkkademyDb>());

            actorRef.Tell(new SetRequest("key", "value"), ActorRefs.NoSender);

            var akkademyDb = actorRef.UnderlyingActor;
            Assert.Equal("value", akkademyDb.Map["key"].ToString());
        }
    }
}
