using System;
using System.Collections.Generic;
using System.Text;

namespace resque
{
    namespace Failure
    {
        public class Base
        {
            public Exception exception { get; set; }
            public Worker worker { get; set; }
            public string queue { get; set; }
            public object payload { get; set; }

            public Base(Exception in_exc, Worker in_wor, String in_queue, Object in_payload)
            {
                exception = in_exc;
                worker = in_wor;
                queue = in_queue;
                payload = in_payload;
            }

            //Declaring these as abstract to force subclass to
            //implement them
            public abstract void save();

            public virtual int count()
            {
                return 0;
            }

            public virtual List<Base> all()
            {
                return new List<Base>();
            }

            public abstract string url();
            public abstract void clear();

            public void log(string message)
            {
                //TODO: Implement worker log function
                //worker.log(message)
            }

        }

    }
}
