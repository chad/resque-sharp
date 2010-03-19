using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using resque;

namespace resque.Failure
{
    public class Redis : Backend
    {
        public Redis(Exception exception, Worker worker, String queue, Object payload)
        {
            this.exception = exception;
            this.worker = worker;
            this.queue = queue;
            this.payload = payload;

        }

        public override void save()
        {
            Dictionary<string, object> data = new Dictionary<string,object>();

            data.Add("failed_at", System.DateTime.Now);
            data.Add("payload", payload);
            data.Add("error", exception.Message);
            data.Add("backtrace", exception.ToString());
            data.Add("worker", worker.ToString());
            data.Add("queue", queue.ToString());

            Resque.redis().RightPush("resque:failed", Resque.encode(data));
        }

        public static int count()
        {
            return Resque.redis().ListLength("resque:failed");
        }

        public static Byte[][] all(int start, int end)
        {
            return Resque.redis().ListRange("resque:failed", start, end);
        }

        public static Byte[][] all()
        {
            return Resque.redis().ListRange("resque:failed", 0, Resque.redis().ListLength("resque:failed"));
        }

        public static string url()
        {
            return Resque.redis().Host;
        }

        //TODO: Redo this to delete the resque:failure queue from the redis object
        public static void clear()
        {
            Resque.redis().Remove("resque:failed");
        }
    }
}
