using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    public class Worker
    {
        string[] queues;
        public Worker(params string[] queues)
        {
            this.queues = queues;
        }

        public void work(int interval)
        {
            work(interval, null);
        }

        public void work(int interval, Func<Job,bool> block)
        {
            try
            {
                startup();
                while (true)
                {
                    Job job = reserve();
                    if (job != null)
                    {
                        process(job, block);
                    }
                    else
                    {
                        if (interval == 0)
                            break;
                        System.Threading.Thread.Sleep(interval * 1000);
                    }


                }
            }
            finally
            {
                unregisterWorker();
            }
        }

        private void unregisterWorker()
        {
            setDoneWorking();
            Resque.redis().RemoveFromSet("resque:workers", workerId());
            Resque.redis().Remove("worker:" + workerId() + ":started");
            // FIXME clear stats
        }

        private void process(Job job, Func<Job, bool> block)
        {
            try
            {
                setWorkingOn(job);
                job.perform();
            }
            catch (Exception e)
            {
                job.fail(e);
                setFailed();
            }
            finally
            {
                if (block != null)
                {
                    block(job);
                }
                setDoneWorking();
            }
        }

        private void setFailed()
        {
            // FIXME
        }

        private void setWorkingOn(Job job)
        {
            job.worker = this;
            string data = Resque.encode(new Dictionary<string, object>() { { "queue", job.queue }, { "run_at", currentTimeFormatted() }, { "payload", job.payload } });
            Resque.redis().Set("resque:worker:" + workerId(), data);
        }

        private void setDoneWorking()
        {
            setProcssed();
            Resque.redis().Remove("resque:worker:" + workerId());
        }

        private void setProcssed()
        {
            //FIXME
        }

        private void startup()
        {
            //pruneDeadWorkers();
            registerWorker();
        }

        private void registerWorker()
        {
            Resque.redis().AddToSet("resque:workers", workerId());
            setStarted();
        }

        private Job reserve()
        {
            foreach(string queue in queues) {
                Job job = Job.Reserve(queue);
                if (job != null)
                {
                    return job;
                }
            }
            return null;
        }

        private void setStarted()
        {
            currentTimeFormatted();
            Resque.redis().Set(new Dictionary<string, byte[]>() { { startedKey(), Encoding.UTF8.GetBytes(currentTimeFormatted()) } });
        }

        private static string currentTimeFormatted()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimeFormatted = currentTime.ToString("ddd MMM dd hh:mm:ss zzzz yyyy");
            return currentTimeFormatted;
        }

        private string startedKey()
        {
            return "resque:worker:" + workerId() + ":started";
        }

        public bool IsWorking()
        {
            return state() == "working";
        }

        public string state()
        {
            return Resque.redis().ContainsKey("resque:worker:" + workerId()) ? "working" : "idle";
        }

        internal string workerId()
        {
            return "FIXME";
        }
    }
}
