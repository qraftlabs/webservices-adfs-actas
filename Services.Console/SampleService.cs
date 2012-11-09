using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.IdentityModel.Claims;

namespace Services.ConsoleHost
{
    public class SampleService : ISampleService
    {
        public void DoWork()
        {
            Console.WriteLine("Service called");

            Console.WriteLine("Authenticated: {0}", Thread.CurrentPrincipal.Identity.IsAuthenticated);
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                foreach (var claim in ((IClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims)
                {
                    Console.WriteLine("\t{0}: {1}", claim.ClaimType, claim.Value);
                }
            }
        }
    }
}
