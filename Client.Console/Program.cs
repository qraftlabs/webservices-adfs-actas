using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Services.SampleServiceClient();

            SecurityToken token = TokenHelper.GetSAML2TokenWithUserNameCredentials(
                                                    user: "Administrator",
                                                    password: "u794Wav5!",
                                                    usernamePasswordEndpoint: "https://test-adfs.auth10.com/adfs/services/trust/13/usernamemixed",
                                                    relyingPartyIdentifier: "urn:my-service");

            TokenHelper.AttachToken(client.ChannelFactory, token);

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
