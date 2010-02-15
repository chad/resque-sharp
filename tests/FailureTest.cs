using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    [TestFixture]
    class FailureTest
    {
        [SetUp]
        public void Init()
        {
            String server = "ec2-184-73-9-230.compute-1.amazonaws.com";
            new Redis(server, 6379).FlushAll(); // This is the IP address of my computer running Redis. 
            Resque.setRedis(new Redis(server, 6379));
        }
    }
}
