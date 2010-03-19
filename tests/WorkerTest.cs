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
        class NUnitConsoleRunner
        {
            [STAThread]
            static void Main(string[] args)
            {
                NUnit.ConsoleRunner.Runner.Main(args);
            }
        }

        private Worker worker;
        string server;

        [SetUp]
        public void Init()
        {
            server = "ec2-184-73-7-218.compute-1.amazonaws.com";
            //string server = "192.168.56.102";

            Resque.setRedis(new Redis(server, 6379));
            Resque.redis().FlushAll();
            worker = new Worker("jobs");
            //Job.create("jobs", "resque.DummyJob", 20, "/tmp");
        }

        [Test]
        public void CanFailJobs()
        {
            Job.create("jobs", "resque.BadJob");
            worker.work(0);
            Assert.AreEqual(1, Resque.failure.count());
        }

        [Test]
        public void CanPeekAtFailedJobs()
        {
            for (int i = 0; i < 10; i++)
            {
                Job.create("jobs", "resque.BadJob");
            }
            worker.work(0);

            Assert.AreEqual(10, Resque.failure.count());

            Byte[][] b = null;

            Assert.AreEqual(Resque.failure.all().Length, 10);

        }

        [Test]
        public void CanClearFailedJobs()
        {
            Job.create("jobs", "resque.BadJob");
            worker.work(0);
            Assert.AreEqual(1, Resque.failure.count());
            Resque.failure.clear();
            Assert.AreEqual(0, Resque.failure.count());
        }

        [Test]
        public void CatchesExceptionalJobs()
        {
            Job.create("jobs", "resque.BadJob");
            Job.create("jobs", "resque.BadJob");
            worker.work(0);
            worker.work(0);
            worker.work(0);

            Assert.AreEqual(2, Resque.failure.count());
        }

        [Test]
        public void CanWorkOnMultipleQueues()
        {
            Job.create("high", "resque.GoodJob");
            Job.create("critical", "resque.GoodJob");

            worker = new Worker(new string[] {"critical", "high" });

            worker.process();
            Assert.AreEqual(Resque.size("high"), 1);
            Assert.AreEqual(Resque.size("critical"), 0);

            worker.process();
            Assert.AreEqual(Resque.size("high"), 0);

        }

        [Test]
        public void CanWorkOnAllQueues()
        {
            Job.create("high", "resque.GoodJob");
            Job.create("critical", "resque.GoodJob");
            Job.create("blahblah", "resque.GoodJob");

            worker = new Worker("*");

            worker.work(0);

            Console.WriteLine(Resque.size("high"));
            Console.WriteLine(Resque.size("critical"));
            Console.WriteLine(Resque.size("blahblah"));

            Assert.AreEqual(Resque.size("high"), 0);
            Assert.AreEqual(Resque.size("critical"), 0);
            Assert.AreEqual(Resque.size("blahblah"), 0);

        }

        [Test]
        public void ProcesesAllQueuesInAlphabeticalOrder()
        {
            Job.create("high", "resque.GoodJob");
            Job.create("critical", "resque.GoodJob");
            Job.create("blahblah", "resque.GoodJob");

            worker = new Worker("*");

            worker.work(0, (List<String> queueList) => { int a; });


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
            /*int random = new System.Random().Next(5, 20);

            for (int i = 0; i < random; i++)
            {
                Job.create("jobs", "resque.BadJob");
            }

            Assert.AreEqual(random, worker.processed());
            */
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
