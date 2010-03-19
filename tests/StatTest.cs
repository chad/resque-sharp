using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace resque
{
    [TestFixture]
    class StatTest
    {
        class NUnitConsoleRunner
        {
            [STAThread]
            static void Main(string[] args)
            {
                NUnit.ConsoleRunner.Runner.Main(args);
            }
        }

        private string server;
        private Object payload;

        [SetUp]
        public void Init()
        {
            // This is the IP address of my computer running Redis. 
            server = "ec2-184-73-7-218.compute-1.amazonaws.com";
            //server = "192.168.56.102";

            Resque.setRedis(new Redis(server, 6379));
            Resque.redis().FlushAll();
        }

        [Test]
        public void canCreateAStat()
        {
            Stat.increment("fakeStat");
            int statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(1, statRetrieveValue);
        }

        [Test]
        public void canCreateAndCreateAndIncrementStat()
        {
            Random rand = new Random(System.DateTime.Now.Second);
            int statExpectValue = rand.Next(5, 20);

            for (int i = 0; i < statExpectValue; i++)
            {
                Stat.increment("fakeStat");
            }

            int statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(statExpectValue, statRetrieveValue);
        }

        [Test]
        public void canCreateStatGreaterThanOne()
        {
            Random rand = new Random(System.DateTime.Now.Second);
            int statExpectValue = rand.Next(5, 20);

            Stat.increment("fakeStat", statExpectValue);

            int statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(statExpectValue, statRetrieveValue);
        }

        [Test]
        public void canDecrementAStat()
        {
            Random rand = new Random(System.DateTime.Now.Second);
            int statExpectValue = rand.Next(5, 20);

            for (int i = 0; i < statExpectValue; i++)
            {
                Stat.increment("fakeStat");
            }

            Stat.decrement("fakeStat");
            int statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(statExpectValue - 1, statRetrieveValue);
        }

        [Test]
        public void canClearStats()
        {
            Random rand = new Random(System.DateTime.Now.Second);
            int statExpectValue = rand.Next(5, 20);

            for (int i = 0; i < statExpectValue; i++)
            {
                Stat.increment("fakeStat");
            }

            int statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(statExpectValue, statRetrieveValue);

            Stat.clear("fakeStat");

            Stat.increment("fakeStat");

            statRetrieveValue = Stat.get("fakeStat");
            Assert.AreEqual(1, statRetrieveValue);
        }

        
    }
}