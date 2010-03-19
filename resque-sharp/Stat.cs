using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resque
{
    public class Stat 
    {
        public Stat()
        {
            throw new NotImplementedException();
        }

        public static int get(String stat)
        {
            return Int32.Parse(Resque.redis().GetString("resque:stat:" + stat));
        }

        public static void increment(String stat, int amt)
        {
            Resque.redis().Increment("resque:stat:" + stat, amt);
        }

        public static void increment(String stat)
        {
            Resque.redis().Increment("resque:stat:" + stat, 1);
        }

        public static void decrement(String stat)
        {
            Resque.redis().Decrement("resque:stat:" + stat);
        }

        public static void clear(String stat)
        {
            Resque.redis().Remove("resque:stat:" + stat);
        }
    }
}
