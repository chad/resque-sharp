using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using resque;

namespace resque.Failure
{
    class Redis : Base
    {
        public void save()
        {
            Dictionary<string, object> data = new Dictionary<string,object>();

            data.Add("failed_at", System.DateTime.Now);
            data.Add("payload", payload);
            data.Add("error", exception.Message);
            data.Add("backtrace", exception.ToString());
            data.Add("worker", worker.ToString());
            data.Add("queue", queue.ToString());

            Resque.Push("failed", Resque.encode(data));

            Resque.redis
            
        }

        public int count()
        {
            return myRedis.count();
        }

        public List<Base> all()
        {
            return myRedis.all();
        }

        public void clear()
        {
            myRedis.delete("resque:failed");
        }
    }
}
