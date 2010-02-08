using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace resque
{
    public class Resque
    {
        private static Redis staticRedis;
        public static void setRedis(Redis redis)
        {
            staticRedis = redis;
        }
        public static Redis redis()
        {
            if (staticRedis == null)
            {
                staticRedis = new Redis();
            }
            return staticRedis;
        }

        public static bool push(string queue, object item)
        {
            watchQueue(queue);
            redis().RightPush("queue:" + queue, encode(item));
            return true;
        }

        private static void watchQueue(string queue)
        {
            redis().AddToSet("queues", "queue");
        }

  
        private static string encode(object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private static object decode(string json)
        {
             return JsonConvert.DeserializeObject<Dictionary<string,object>>(json);
        }
        private static object decode(byte[] json)
        {
            return decode(Encoding.UTF8.GetString(json));
        }

        public static Job Reserve(string queue)
        {
            return Job.Reserve(queue);
        }


        internal static Dictionary<string,object> Pop(string queue)
        {
            return (Dictionary<string,object>)decode(redis().LeftPop("queue:" + queue));
        }

        public static int size(string queue)
        {
            return redis().ListLength("queue:" + queue);
        }

        public static bool enqueue(string className, params object[] args)
        {
            Type workerType = Type.GetType(className);
            System.Reflection.MethodInfo methodInfo = workerType.GetMethod("queue", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
            string queue = (string)methodInfo.Invoke(null, null);
            return Job.create(queue, className, args);
        }
    }
}
