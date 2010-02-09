using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace resque
{

    [TestFixture]
    public class ResqueTest
    {
        [SetUp]
        public void Init()
        {
            new Redis("192.168.1.119", 6379).FlushAll(); // This is the IP address of my computer running Redis. 
            Resque.setRedis(new Redis("192.168.1.119", 6379));
            Resque.Push("people", new Dictionary<string, string>(){{"name", "chris"}});
            Resque.Push("people", new Dictionary<string, string>(){{"name", "bob"}});
            Resque.Push("people", new Dictionary<string, string>(){{"name", "mark"}});
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
            //Job.create("jobs", "dummy-job", 20, "/tmp"); FIXME NEED TO DEAL WITH THIS
            Job.create("jobs", "resque.DummyJob", 20, "/tmp");
            Job job = Resque.Reserve("jobs");
            Assert.AreEqual("resque.DummyJob", job.PayloadClass().FullName);
            var num = job.args()[0];
            Assert.AreEqual(20, num);
            Assert.That("/tmp", Is.EqualTo(job.args()[1]));
        }

        [Test]
        public void CanReQueueJobs()
        {
            Job.create("jobs", "resque.DummyJob", 20, "/tmp");
            Job job = Resque.Reserve("jobs");
            job.recreate();
            Assert.That(job, Is.EqualTo(Resque.Reserve("jobs")));
        }

        [Test]
        public void CanAskResqueForQueueSize()
        {
            Assert.That(0, Is.EqualTo(Resque.size("a_queue")));
            Job.create("a_queue", "resque.DummyJob", 1, "asdf");
            Assert.That(1, Is.EqualTo(Resque.size("a_queue")));
        }

        [Test]
        public void CanPutJobsOnTheQueueByAskingWhichQueueTheyAreInterestedIn()
        {
            Assert.That(0, Is.EqualTo(Resque.size("tester")));
            Assert.IsTrue(Resque.enqueue("resque.DummyJob", 20, "/tmp"));
            Assert.IsTrue(Resque.enqueue("resque.DummyJob", 20, "/tmp"));

            Job job = Resque.Reserve("tester");

            Assert.That(20, Is.EqualTo(job.args()[0]));
            Assert.That("/tmp", Is.EqualTo(job.args()[1]));
        }


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

        [Test]
        public void QueueMustBeInferrable() {
            Assert.That(
                new TestDelegate(EnqueueUninferrableJob), 
                Throws.TypeOf<resque.NoQueueError>()
                );
        }

        [Test]
        public void CanPutItemsOnAQueue()
        {
            Dictionary<string, string> person = new Dictionary<string, string>();
            person.Add("name", "chris");
            Assert.That(Resque.Push("people", person), Is.True);
        }

        [Test]
        public void CanPullItemsOffAQueue()
        {
            Assert.That("chris", Is.EqualTo(((Dictionary<string,object>)Resque.Pop("people"))["name"]));
            Assert.That("bob", Is.EqualTo(((Dictionary<string, object>)Resque.Pop("people"))["name"]));
            Assert.That("mark", Is.EqualTo(((Dictionary<string, object>)Resque.Pop("people"))["name"]));
            Assert.That(Resque.Pop("people"), Is.Null);
        }

        [Test]
        public void KnowsHowBigAQueueIs()
        {
            Assert.That(Resque.size("people"), Is.EqualTo(3));
            Assert.That("chris", Is.EqualTo(((Dictionary<string, object>)Resque.Pop("people"))["name"]));
            Assert.That(Resque.size("people"), Is.EqualTo(2));
            Resque.Pop("people");
            Resque.Pop("people");
            Assert.That(Resque.size("people"), Is.EqualTo(0));
        }

        [Test]
        public void CanPeekAtAQueue()
        {
            Assert.That("chris", Is.EqualTo(((Dictionary<string, object>)Resque.Peek("people"))["name"]));
            Assert.That(Resque.size("people"), Is.EqualTo(3));
        }

        [Test]
        public void CanPeekAtMultipleItemsOnQueue()
        {
            ArrayList result = Resque.Peek("people", 1, 1);
            Assert.That("bob", Is.EqualTo((((Dictionary<string, object>)result[0]))["name"]));

            result = Resque.Peek("people", 1, 2);
            Assert.That(((Dictionary<string, object>)result[0])["name"], Is.EqualTo("bob"));
            Assert.That(((Dictionary<string, object>)result[1])["name"], Is.EqualTo("mark"));

            result = Resque.Peek("people", 0, 2);
            Assert.That(((Dictionary<string, object>)result[0])["name"], Is.EqualTo("chris"));
            Assert.That(((Dictionary<string, object>)result[1])["name"], Is.EqualTo("bob"));

            result = Resque.Peek("people", 2, 1);
            Assert.That(((Dictionary<string, object>)result[0])["name"], Is.EqualTo("mark"));
            Assert.That(Resque.Peek("people", 3), Is.Null);
        }

        [Test]
        public void KnowsWhatQuestsItIsManaging()
        {
            Assert.That(Resque.queues(), Is.EqualTo(new string[1]{"people"}));
            Resque.Push("cars", new Dictionary<string, string>(){{"make", "bmw"}});
            Assert.That(Resque.queues(), Is.EqualTo(new string[2] {"cars", "people" }));
        }


        [Test]
        public void QueuesAreAlwaysAList()
        {
            Resque.redis().FlushAll();
            Assert.That(Resque.queues(), Is.EqualTo(new string[0]));
        }

        [Test]
        public void CanDeleteAQueue()
        {
            Resque.Push("cars", new Dictionary<string, string>() { { "make", "bmw" } });
            Assert.That(Resque.queues(), Is.EqualTo(new string[2] { "cars", "people" }));
            Resque.RemoveQueue("people");
            Assert.That(Resque.queues(), Is.EqualTo(new string[1] { "cars" }));
        }


        internal void EnqueueUninferrableJob()
        {
            Resque.enqueue("resque.UninferrableInvalidJob", 123);
        }

    }
}
