using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;

namespace resque
{
    public class Worker
    {
        string[] queues;
        public string id { get; set; }

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

        public void unregisterWorker()
        {
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
            //catch (Exception e)
            //{
            //    job.fail(e);
            //    setFailed();
            //}
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
        public object IsIdle()
        {
            return state() == "idle";
        }
        public string state()
        {
            return Resque.redis().ContainsKey("resque:worker:" + workerId()) ? "working" : "idle";
        }

        public string workerId()
        {
            if(id == null) {
                var sb = new StringBuilder();
                sb.Append(hostname());
                sb.Append(":");
                sb.Append(currentProcessIdAsString());
                sb.Append(":");
                sb.Append(String.Join(",", queues));
                id = sb.ToString();
            }
            return id;
        }

        internal static Worker[] all()
        {
            var workers = from id in Resque.redis().GetMembersOfSet("resque:workers")
                               select find(Encoding.UTF8.GetString(id));

            return workers.ToArray<Worker>();
        }

        public static Worker find(string id)
        {
            if (isExisting(id))
            {
                string[] components = id.Split(":".ToCharArray());
                string queuesPacked = components[components.Length - 1];
                string[] queues = queuesPacked.Split(",".ToCharArray());
                Worker worker = new Worker(queues);
                worker.id = id;
                return worker;
            }
            else
            {
                return null;
            }
        }

        public static bool isExisting(string id)
        {
            return Resque.redis().IsMemberOfSet("resque:workers", id);
        }

        internal static Worker[] working()
        {
            Worker[] workers = all();
            if (workers.Length == 0)
                return workers;
            string[] names = (from worker in workers
                             select "resque:worker:" + worker.workerId()).ToArray<string>();
            string[] redisValues = (from bytes in Resque.redis().GetKeys(names)
                                    select bytes == null ? null : Encoding.UTF8.GetString(bytes)).ToArray<string>(); ;
            List<Worker> results = new List<Worker>();
            for (int i = 0; i < names.Length; i++)
            {
                if (!String.IsNullOrEmpty(redisValues[i]))
                {
                    results.Add(find(Regex.Replace(names[i], "resque:worker:", "")));
                }
            }
            return results.ToArray();
        }

        private string hostname()
        {
            return Dns.GetHostName();
        }

        private string currentProcessIdAsString()
        {
            int processID = System.Diagnostics.Process.GetCurrentProcess().Id;
            return processID.ToString();
        }
    }
}
