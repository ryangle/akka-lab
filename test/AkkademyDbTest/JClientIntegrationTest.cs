using System;
using System.Collections.Generic;
using System.Text;
using Akka.TestKit.Xunit2;
using AkkademydbClient;
using Xunit;

namespace AkkademyDb.Test
{
    public class JClientIntegrationTest : TestKit
    {
        private NClient client = new NClient("127.0.0.1:2552");
        [Fact]
        public void ItShouldSetRecord()
        {
            client.Set("123", 123);
            var result = (int)client.Get("123").Result;
            Assert.Equal(123, result);
        }
    }
}
