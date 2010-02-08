using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace resque
{

    [TestFixture]
    public class ResqueTest
    {
        [SetUp]
        public void Init()
        {
            new Redis("192.168.1.119", 6379).FlushAll();
        }
        [Test]
        public void CanPutJobsOnAQueue()
        {
            Assert.IsTrue(Job.create("jobs", "DummyJob", 20, "/tmp"));
            Assert.IsTrue(Job.create("jobs", "DummyJob", 20, "/tmp"));
        }

        [Test]
        public void CanGrabJobsOffAQueue()
        {
            Resque.setRedis(new Redis("192.168.1.119", 6379)); // FIXME
            //Job.create("jobs", "dummy-job", 20, "/tmp"); FIXME NEED TO DEAL WITH THIS
            Job.create("jobs", "resque.DummyJob", 20, "/tmp");
            Job job = Resque.Reserve("jobs");
            Assert.AreEqual("resque.DummyJob", job.PayloadClass().FullName);
            var num = job.args()[0];
            Assert.AreEqual(20, num);
            Assert.That("/tmp", Is.EqualTo(job.args()[1]));
        }

        //[Test]
        //public void CanReQueueJobs()
        //{
        //    Job.create("jobs", "resque.DummyJob", 20, "/tmp");
        //    Job job = Resque.Reserve("jobs");
        //    job.recreate();
        //    Assert.That(job, Is.EqualTo(Resque.Reserve("jobs")));
        //}

        [Test]
        public void CanTestForEquality()
        {
            Assert.IsTrue(Job.create("jobs", "resque.DummyJob", 20, "/tmp"));
            Assert.IsTrue(Job.create("jobs", "resque.DummyJob", 20, "/tmp"));
            //Assert.IsTrue(Job.create("jobs", "dummy-job", 20, "/tmp"));  NEED TO  MAKE THIS WORK
            Assert.That(Resque.Reserve("jobs"), Is.EqualTo(Resque.Reserve("jobs")));

            Assert.IsTrue(Job.create("jobs", "resque.NotDummyJob", 20, "/tmp"));
            Assert.IsTrue(Job.create("jobs", "resque.DummyJob", 20, "/tmp"));
            Assert.That(Resque.Reserve("jobs"), Is.Not.EqualTo(Resque.Reserve("jobs")));

            Assert.IsTrue(Job.create("jobs", "resque.DummyJob", 20, "/tmp"));
            Assert.IsTrue(Job.create("jobs", "resque.DummyJob", 30, "/tmp"));
            Assert.That(Resque.Reserve("jobs"), Is.Not.EqualTo(Resque.Reserve("jobs")));

        }

    }
}
