using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Xunit;

namespace akkademy_db.Test
{
    public class AkkademyDbTest:TestKit
    {
        [Fact]
        public void ItShouldPlaceKeyValueFromSetMessageIntoMap()
        {
            var actorRef = Sys.ActorOf<AkkademyDb>();

            var probe = CreateTestProbe();

            actorRef.Tell(new SetRequest("key", "value"), TestActor);
            //AkkademyDb akkademyDb = actorRef.underlyingActor();
            //Assert.Equal(akkademyDb.map.get("key"), "value");
        }
    }
}
