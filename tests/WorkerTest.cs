using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace resque
{
    [TestFixture]
    public class WorkerTest
    {
        private Worker worker;
        [SetUp]
        public void Init()
        {
            Resque.setRedis(new Redis("192.168.1.119", 6379));
            Resque.redis().FlushAll();
            worker = new Worker("jobs");
            Job.create("jobs", "resque.DummyJob", 20, "/tmp");
        }

        //[Test]
        //public void CanFailJobs()
        //{
        //    Job.create("jobs", "resque.BadJob");
        //    worker.work(0);
        //}

        [Test]
        public void KnowsWhenItsWorking()
        {
            worker.work(0, (Job Job) => {Assert.That(worker.IsWorking(), Is.True); return true;});
        }

    }
}
