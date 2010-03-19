using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using resque;

class DummyJob
{
    public static string queue()
    {
        return "jobs";
    }
    public static void perform(params object[] args)
    {
        Console.WriteLine("This is the dummy job reporting in");
    }
}

namespace ExampleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Type t = typeof(DummyJob);
            Console.WriteLine(t.AssemblyQualifiedName);
            string assemblyQualification = ", ExampleRunner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            Resque.setAssemblyQualifier(assemblyQualification);
            String server = "192.168.1.7";
            Resque.setRedis(new Redis(server, 6379));
            Job.create("jobs", "DummyJob", "foo", 20, "bar");
            Worker w = new Worker("*");
            w.work(1);
        }
    }
}
