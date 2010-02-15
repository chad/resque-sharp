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
            String server = "ec2-184-73-9-230.compute-1.amazonaws.com";
            Resque.setRedis(new Redis(server, 6379));
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
        public void CanPeekAtFailedJobs()
        {
        }

        [Test]
        public void CanClearFailedJobs()
        {
        }

        [Test]
        public void CatchesExceptionalJobs()
        {
        }

        [Test]
        public void CanWorkOnMultipleQueues()
        {
        }

        [Test]
        public void CanWorkOnAllQueues()
        {
        }

        [Test]
        public void ProcesesAllQueuesInAlphabeticalOrder()
        {
        }

        [Test]
        public void HasAUniqueId()
        {
        }

        [Test]
        public void ComplainsIfNoQueuesAreGiven()
        {
        }

        [Test]
        public void FailsIfAJobClassHasNoPerformMethod()
        {
        }


        [Test]
        public void KnowsWhenItsWorking()
        {
            worker.work(0, (Job Job) => {Assert.That(worker.IsWorking(), Is.True); return true;});
        }

        [Test]
        public void KnowsWhenItIsIdle()
        {
            worker.work(0);
            Assert.That(worker.IsIdle(), Is.True);
        }

        [Test]
        public void KnowsWhoIsWorking()
        {
            worker.work(0, 
                (Job job) => { 
                    Assert.That(Resque.working()[0].workerId(), Is.EqualTo(worker.workerId())); return true; 
                });
        }


        [Test]
        public void KeepsTrackOfHowManyJobsItHasProcessed()
        {
        }

        [Test]
        public void KeepsTrackOfHowManyFailuresItHasSeen()
        {
        }


        [Test]
        public void StatsAreErasedWhenTheWorkerGoesAway()
        {
        }

        [Test]
        public void KnowsWhenItIsStarted()
        {
        }

        [Test]
        public void KnowsWhetherItExistsOrNot()
        {
        }

        [Test]
        public void CanBeFound()
        {
            worker.work(0, (Job job) =>
            {
                Assert.That(Worker.find(worker.workerId()).workerId(), Is.EqualTo(worker.workerId()));
                return true;
            });
        }

        [Test]
        public void InsertsItselfIntoTheWorkersListOnStartup()
        {
            worker.work(0,
                (Job job) =>
                {
                    Assert.That(Resque.workers()[0].workerId(), Is.EqualTo(worker.workerId())); return true;
                });
        }

        [Test]
        public void RemovesItselfFromTheWorkersListOnShutdown()
        {
            worker.work(0,
                (Job job) =>
                {
                    Assert.That(Resque.workers()[0].workerId(), Is.EqualTo(worker.workerId())); return true;
                });
            Assert.That(Resque.workers().Length, Is.EqualTo(0));
        }

        [Test]
        public void RemovesWorkerWithStringifiedId()
        {
            worker.work(0,
                (Job job) =>
                {
                    var workerId = Resque.workers()[0].workerId();
                    Resque.removeWorker(workerId);
                    Assert.That(Resque.workers().Length, Is.EqualTo(0));
                    return true;
                });
        }

        [Test]
        public void ClearsItsStatusWhenNotWorkingOnAnything()
        {
        }


        //[Test]
        //public void RecordsWhatItIsWorkingOn()
        //{
        //    worker.work(0,
        //        (Job job) =>
        //        {
        //            Dictionary<string, object> data = worker.job();
        //            Dictionary<string, object> payload = (Dictionary<string, object>)data["payload"];
        //           // throw new Exception(String.Join(",", data.Keys.ToArray<string>()));
        //            Assert.That(data["class"], Is.EqualTo("resque.DummyJob"));
        //            return true;
        //        });
        //}


    }
}
