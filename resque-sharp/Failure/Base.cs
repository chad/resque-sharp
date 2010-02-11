using System;
using System.Collections.Generic;
using System.Text;

namespace resque
{
    namespace Failure
    {
        public abstract class Base
        {
            public Exception exception { get; set; }
            public Worker worker { get; set; }
            public string queue { get; set; }
            public object payload { get; set; }

            public Base(Exception exception, Worker worker, String queue, Object payload)
            {
                this.exception = exception;
                this.worker = worker;
                this.queue = queue;
                this.payload = payload;
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
