using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

            var client = new Services.SampleServiceClient();

            Console.WriteLine("Press ENTER to call the service");
            Console.ReadLine();

            SecurityToken token = TokenHelper.GetSAML2TokenWithUserNameCredentials(
                                                    user: "REPLACE_WITH_USER",
                                                    password: "REPLACE_WITH_PASSWORD",
                                                    usernamePasswordEndpoint: "https://<REPLACE_WITH_ADFS>/adfs/services/trust/13/usernamemixed",
                                                    relyingPartyIdentifier: "REPLACE_WITH_RELYINGPARTY_IDENTIFIER");

            Console.WriteLine("Getting token from ADFS...");
            TokenHelper.AttachToken(client.ChannelFactory, token);

            
            try
            {
                Console.WriteLine("Calling service...");
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
