using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace resque
{
    [TestFixture]
    class FailureTest
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
        private Failure.Redis myRedis;
        private String testString = "failed";
        private Object payload;

        [SetUp]
        public void Init()
        {
            // This is the IP address of my computer running Redis. 
            server = "ec2-184-73-7-218.compute-1.amazonaws.com";
            
            Resque.setRedis(new Redis(server, 6379));
            Resque.redis().FlushAll(); 

            Exception ex = new Exception(testString);
            Worker worker = new Worker();
            String queue = testString;
            payload = Encoding.UTF8.GetBytes(testString);
            
            myRedis = new Failure.Redis(ex, worker, queue, payload);
        }

        [Test]
        public void CanCreateFailure()
        {

            Assert.AreEqual(testString, myRedis.exception.Message);
            Assert.AreEqual(testString, myRedis.queue);

            Object temp = (Byte[]) myRedis.payload;

            Assert.AreEqual(temp, payload);

        }

        [Test]
        public void CanGetURL()
        {
            Assert.AreEqual(myRedis.url(), server);
        }

        [Test]
        public void CanCheckEmptyQueue()
        {
            Assert.AreEqual(0, myRedis.count());
        }

        [Test]
        public void CanSaveOnItemToQueue()
        {
            myRedis.save();

            int count = myRedis.count();
            Assert.AreEqual(1, count);

        }

        [Test]
        public void CanSaveRandomNumberOfItemsToQueue()
        {
            int random = new System.Random().Next(5, 20);

            for (int i = 0; i < random; i++)
            {
                myRedis.save();
            }

            Assert.AreEqual(random, myRedis.count());
        }

        [Test]
        public void CanClear()
        {
            int randNumOfJobs = new System.Random().Next(5, 20);

            for (int i = 0; i < randNumOfJobs; i++)
            {
                myRedis.save();
            }

            Assert.AreEqual(randNumOfJobs, myRedis.count());

            myRedis.clear();

            Assert.AreEqual(0, myRedis.count());
        }

        [Test]
        public void CanRetrieveAllKeys()
        {
            int randNumOfJobs = new System.Random().Next(5, 20);

            for (int i = 0; i < randNumOfJobs; i++)
            {
                myRedis.save();
            }

            Byte[][] allKeys = myRedis.all(0, randNumOfJobs);

            Assert.AreEqual(allKeys.Length, randNumOfJobs);

        }



    }
}
