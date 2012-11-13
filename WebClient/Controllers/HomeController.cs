using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using System.ServiceModel.Security;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var userToken = ((IClaimsIdentity)Thread.CurrentPrincipal.Identity).BootstrapToken;
            var token = TokenHelper.GetSAML2ActAsTokenWithFunctionalAccount(
                actAsToken: userToken, 
                user: "REPALCE_WITH_SERVICE_ACCOUNT",
                password: "REPALCE_WITH_SERVICE_ACCOUNT_PASSWORD",
                usernamePasswordEndpoint: "https://REPLACE_WITH_ADFS/adfs/services/trust/13/usernamemixed",
                relyingParty: "REPLACE_WITH_RELYINGPARTY_IDENTIFIER");


            var client = new Services.SampleServiceClient();
            TokenHelper.AttachToken(client.ChannelFactory, token);

            try
            {
                client.DoWork();
                ViewBag.Message = "Service was called";
            }
            catch (SecurityAccessDeniedException ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
