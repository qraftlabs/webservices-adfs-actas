using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Collections.ObjectModel;
using System.ServiceModel.Security;

namespace Services.ConsoleHost
{
    public class ValidateSamlToken : ServiceAuthorizationManager
    {
        public override bool CheckAccess(OperationContext operationContext)
        {
            int index = operationContext.RequestContext.RequestMessage.Headers.FindHeader("Security", WSSecurity10Constants.Namespace);
            if (index < 0)
            {
                return false;
            }

            var securityHeader = operationContext.RequestContext.RequestMessage.Headers.GetReaderAtHeader(index);

            var configuration = new ServiceConfiguration(true);

            securityHeader.Read();
            if (!configuration.SecurityTokenHandlers.CanReadToken(securityHeader))
                return false;

            if (configuration.ServiceCertificate != null)
            {
                SecurityToken xtoken;

                using (var xprovider = new X509SecurityTokenProvider(configuration.ServiceCertificate))
                {
                    xtoken = xprovider.GetToken(new TimeSpan(10, 1, 1));
                }

                var outOfBandTokens = new Collection<SecurityToken>();
                outOfBandTokens.Add(xtoken);

                configuration.SecurityTokenHandlers[typeof(EncryptedSecurityToken)].Configuration.ServiceTokenResolver =
                    SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new ReadOnlyCollection<SecurityToken>(outOfBandTokens), false);
            }

            SecurityToken token = configuration.SecurityTokenHandlers.ReadToken(securityHeader);
            if (configuration.CertificateValidationMode == X509CertificateValidationMode.None)
            {
                configuration.CertificateValidator = X509CertificateValidator.None;
            }

            var claimsCollection = configuration.SecurityTokenHandlers.ValidateToken(token);

            IClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsCollection);

            SetPrincipal(operationContext, claimsPrincipal);
            
            return true;
        }

        private void SetPrincipal(OperationContext operationContext, IClaimsPrincipal principal)
        {
            var properties = operationContext.ServiceSecurityContext.AuthorizationContext.Properties;

            if (!properties.ContainsKey("Principal"))
            {
                properties.Add("Principal", principal);
            }
            else
            {
                properties["Principal"] = principal;
            }
        }
    }
}
