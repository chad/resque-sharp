using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    public class Job
    {
        Dictionary<string, object> payload;
        string queue;

        public Job(string queue, Dictionary<string, object> payload)
        {
            this.queue = queue;
            this.payload = payload;
        }

        public Type PayloadClass()
        {
            return Type.GetType((string)payload["class"], true); 
        }

        public static bool create(string queue, string className, params object[] args)
        {
            Dictionary<String,Object> dictionary = new Dictionary<String,Object>();
            dictionary.Add("class", className);
            dictionary.Add("args", args);
            Resque.push(queue, dictionary);
            return true;
        }

        internal static Job Reserve(string queue)
        {
            Dictionary<string,object> payload = Resque.Pop(queue);
            if (payload == null)
                return null;
            return new Job(queue, payload);

        }

        internal void perform()
        {
            //Type type = Type.GetType("resque.DummyJob", true);
            //return (Job)Activator.CreateInstance(type);
        }

        public object[] args()
        {
            return (object[])payload["args"];
        }
    }

    public class DummyJob : Job
    {
        public DummyJob(string queue, Dictionary<string,object> dictionary) : base(queue, dictionary)
        {
          
        }
        // for testing
    }
}
