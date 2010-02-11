using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    namespace Failure
    {
        class Failure
        {
            private Base backend;

            //TODO: Not sure if this is right at all:
            //The Ruby version passes in a hash and passes named fields to the base constructor
            //Does this have to be persisted somehow?
            public Failure(Base in_base)
            {
                backend = in_base;
            }

            public int count()
            {
                return backend.count();
            }

            //TODO: Make a version of this method for paginating results
            public List<Base> all()
            {
                return backend.all();
            }

            public string url()
            {
                return backend.url();
            }

            public void clear()
            {
                backend.clear();
            }


        }
    }
}
