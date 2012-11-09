using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Services.SampleServiceClient();

            Console.WriteLine("Press ENTER to call the service");
            Console.ReadLine();

            try
            {
                client.DoWork();
            }
            catch (SecurityAccessDeniedException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

        }
    }
}
