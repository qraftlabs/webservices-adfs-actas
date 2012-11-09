using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Services.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(SampleService));
            host.Open();

            Console.WriteLine("Service running on {0}", host.Description.Endpoints[0].Address);
            Console.WriteLine("Press ENTER to close it");
            Console.ReadLine();
        }
    }
}
