using System;
using System.Collections.Generic;
using System.Text;

namespace resque
{
    namespace Failure
    {
        public abstract class Backend
        {
            public Exception exception { get; set; }
            public Worker worker { get; set; }
            public string queue { get; set; }
            public object payload { get; set; }

            public Backend(Exception exception, Worker worker, String queue, Object payload)
            {
                this.exception = exception;
                this.worker = worker;
                this.queue = queue;
                this.payload = payload;
            }

            public Backend()
            {
                this.exception = null;
                this.worker = null;
                this.queue = null;
                this.payload = null;
            }

            //Declaring these as abstract to force subclass to
            //implement them
            public abstract void save();

            //FIXME: Temporarily commenting out, figure out correct keywords
            //public abstract string url();
            //public abstract void clear();
            //public virtual int count()
            //{
            //    return 0;
            //}
            /*public virtual Byte[][] all()
            {
                return new Byte[][];
            }*/
            //=======FIXME========

            public void log(string message)
            {
                //TODO: Implement worker log function
                //worker.log(message)
            }

        }

    }
}
