using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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

            public Type create(Exception exception, Worker worker, String queue, Object payload)
            {
                Activator.CreateInstance(backendType, exception, worker, queue, payload);
                return backendType;
            }

            public int count()
            {
                return (int)invokeOnBackend("count");
            }

            //TODO: Make a version of this method for paginating results
            public Byte[][] all()
            {
                return (Byte[][])invokeOnBackend("all");
            }

            public Byte[][] all(int start, int end)
            {
                object[] param = new object[] { start, end };
                return (Byte[][])invokeOnBackend("all", param);
            }

            public string url()
            {
                return (string)invokeOnBackend("url");  
            }

            public void clear()
            {
                invokeOnBackend("clear");
            }

            public object invokeOnBackend(string methodName, params object[] args) 
            {
                Type[] argTypes = new Type[args.Length];

                for(int i = 0; i < args.Length; i++)
                {
                    argTypes[i] = args[i].GetType();
                }
  
                System.Reflection.MethodInfo methodInfo = backendType.GetMethod(methodName, argTypes);
                if (methodInfo == null)
                    throw new NotImplementedException();
                return methodInfo.Invoke(null, args);

            }


        }
    }
}
