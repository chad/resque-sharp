using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    namespace Failure
    {
        public class Failure
        {
            private Type backendType;

            public Failure(Type backendType)
            {
                this.backendType = backendType;
            }

            public void create(Exception exception, Worker worker, String queue, Object payload)
            {
                Activator.CreateInstance(backendType, exception, worker, queue, payload);
            }

            public int count()
            {
                // invoke static method count
                //return backend.count();
                return (int)invokeOnBackend("count");
            }

            //TODO: Make a version of this method for paginating results
            public List<Backend> all()
            {
                return (List<Backend>)invokeOnBackend("all");
            }

            public string url()
            {
                return (string)invokeOnBackend("url");
            }

            public void clear()
            {
                invokeOnBackend("clear");
            }

            public object invokeOnBackend(string methodName, params object[] args) {
                System.Reflection.MethodInfo methodInfo = backendType.GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
                if (methodInfo == null)
                    throw new NotImplementedException();
       
                return methodInfo.Invoke(null, args);
            }

        }
    }
}
