using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

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

        public ArrayList args()
        {
            ArrayList list = new ArrayList();
            JArray args = (JArray)payload["args"];
                foreach (JValue o in args)
                {
                    list.Add(o.Value);
                }
               return list;
            }



        public void recreate()
        {
            Job.create(queue, PayloadClass().FullName, args());
        }
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (this.GetType() != other.GetType())
                return false;
            Job job = (Job)other;
            return (this.queue == job.queue && this.PayloadClass() == job.PayloadClass() && arrayListElementsAreEqual(args(), job.args()));
        }

        private bool arrayListElementsAreEqual(ArrayList list, ArrayList otherList)
        {
            if (list.Count != otherList.Count)
            {
                return false;
            }
            int i = 0;
            foreach (object o in list)
            {
                if (!o.Equals(otherList[i]))
                {
                    return false;
                }
                i++;
            }
            return true;
        }
    }


    // FIXME: These shouldn't have to be here. Need to figure out how to cleanly load from multiple assemblies dynamically
    public class DummyJob
    {
        //public DummyJob(string queue, Dictionary<string,object> dictionary) : base(queue, dictionary)
        //{
          
        //}
        // for testing
    }
    public class NotDummyJob
    {
 
    }

}
