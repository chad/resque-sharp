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

    }
}
