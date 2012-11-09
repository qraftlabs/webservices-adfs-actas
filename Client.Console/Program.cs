using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Services.SampleServiceClient();

            Console.WriteLine("Press ENTER to call the service");
            Console.ReadLine();

            client.DoWork();
        }
    }
}
