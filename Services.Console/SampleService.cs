using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services.ConsoleHost
{
    public class SampleService : ISampleService
    {
        public void DoWork()
        {
            Console.WriteLine("service called");
        }
    }
}
