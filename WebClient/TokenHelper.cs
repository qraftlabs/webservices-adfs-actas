using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.SecurityTokenService;
using System.Configuration;
using System.ServiceModel.Security;

namespace WebClient
{
    public class TokenHelper
    {
        public static SecurityToken GetSAML2TokenWithKerberosCredentials(string kerberosEndpoint = null, string servicePrincipalName = null, string relyingPartyIdentifier = null, bool encryptedToken = false)
        {
            var rst = BuildRequestSecurityToken(relyingPartyIdentifier, encryptedToken, Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11);

            return GetSAMLTokenWithKerberosCredentials(kerberosEndpoint, servicePrincipalName, relyingPartyIdentifier, rst);
        }

        public static SecurityToken GetSAML11TokenWithKerberosCredentials(string kerberosEndpoint = null, string servicePrincipalName = null, string relyingPartyIdentifier = null, bool encryptedToken = false)
        {
            var rst = BuildRequestSecurityToken(relyingPartyIdentifier, encryptedToken, Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml11TokenProfile11);

            return GetSAMLTokenWithKerberosCredentials(kerberosEndpoint, servicePrincipalName, relyingPartyIdentifier, rst);
        }

        private static SecurityToken GetSAMLTokenWithKerberosCredentials(string kerberosEndpoint = null, string servicePrincipalName = null, string relyingPartyIdentifier = null, RequestSecurityToken rst = null)
        {
            var stsEndpoint = kerberosEndpoint ?? ConfigurationManager.AppSettings["kerberosEndpoint"];
            var spnIdentity = servicePrincipalName ?? ConfigurationManager.AppSettings["servicePrincipalName"];
            string serviceEndpoint = relyingPartyIdentifier ?? ConfigurationManager.AppSettings["relyingPartyIdentifier"];

            using (var factory = new WSTrustChannelFactory(GetWindowsWSTrustBinding(), new EndpointAddress(new Uri(stsEndpoint),
              EndpointIdentity.CreateSpnIdentity(spnIdentity), new AddressHeaderCollection())))
            {
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                factory.TrustVersion = TrustVersion.WSTrust13;

                WSTrustChannel channel = null;

                try
                {
                    if (rst == null)
                    {
                        rst = BuildRequestSecurityToken();
                    }

                    channel = (WSTrustChannel)factory.CreateChannel();

                    return channel.Issue(rst);
                }
                finally
                {
                    if (channel != null)
                    {
                        channel.Abort();
                    }

                    factory.Abort();
                }
            }
        }

        public static SecurityToken GetSAML2TokenWithUserNameCredentials(string user, string password, string usernamePasswordEndpoint = null, bool encryptedToken = false, string relyingPartyIdentifier = null)
        {
            var rst = BuildRequestSecurityToken(relyingPartyIdentifier, encryptedToken, Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11);

            return GetSAMLTokenWithUserNameCredentials(user, password, usernamePasswordEndpoint, relyingPartyIdentifier, rst);
        }

        public static SecurityToken GetSAML11TokenWithUserNameCredentials(string user, string password, string usernamePasswordEndpoint = null, bool encryptedToken = false, string relyingPartyIdentifier = null)
        {
            var rst = BuildRequestSecurityToken(relyingPartyIdentifier, encryptedToken, Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml11TokenProfile11);

            return GetSAMLTokenWithUserNameCredentials(user, password, usernamePasswordEndpoint, relyingPartyIdentifier, rst);
        }

        private static SecurityToken GetSAMLTokenWithUserNameCredentials(string user, string password, string usernamePasswordEndpoint = null, string relyingParty = null, RequestSecurityToken rst = null)
        {
            Uri stsEndpoint = new Uri(usernamePasswordEndpoint ?? ConfigurationManager.AppSettings["usernamePasswordEndpoint"]);

            EndpointAddress endpointAddress;
            if (stsEndpoint.Host.Contains("localhost"))
            {
                endpointAddress = new EndpointAddress(stsEndpoint, new DnsEndpointIdentity("SelfSTS"), new AddressHeaderCollection());
            }
            else
            {
                endpointAddress = new EndpointAddress(stsEndpoint);
            }

            Binding binding = stsEndpoint.Scheme == "https" ? GetUserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential) : GetUserNameWSTrustBinding(SecurityMode.Message);
            using (var factory = new WSTrustChannelFactory(binding, endpointAddress))
            {
                factory.Credentials.UserName.UserName = user;
                factory.Credentials.UserName.Password = password;
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                factory.TrustVersion = TrustVersion.WSTrust13;

                WSTrustChannel channel = null;

                try
                {
                    if (rst == null)
                    {
                        rst = BuildRequestSecurityToken();
                    }

                    channel = (WSTrustChannel)factory.CreateChannel();

                    return channel.Issue(rst);
                }
                finally
                {
                    if (channel != null)
                    {
                        channel.Abort();
                    }

                    factory.Abort();
                }
            }
        }

        public static SecurityToken GetSAML2ActAsTokenWithFunctionalAccount(SecurityToken actAsToken, string user, string password, string usernamePasswordEndpoint = null, bool encryptedToken = false, string relyingParty = null)
        {
            var rst = BuildRequestSecurityToken(relyingParty, encryptedToken, Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11, actAsToken);

            return GetSAMLTokenWithUserNameCredentials(user, password, usernamePasswordEndpoint, relyingParty, rst);
        }

        private static RequestSecurityToken BuildRequestSecurityToken(string relyingPartyIdentifier = null, bool encryptedToken = false, string tokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11, SecurityToken actAsToken = null)
        {
            string serviceEndpoint = relyingPartyIdentifier ?? ConfigurationManager.AppSettings["relyingPartyIdentifier"];

            return new RequestSecurityToken
            {
                RequestType = WSTrust13Constants.RequestTypes.Issue,
                AppliesTo = new EndpointAddress(serviceEndpoint),
                KeyType = encryptedToken ? KeyTypes.Symmetric : KeyTypes.Bearer,
                TokenType = tokenType,
                ActAs = actAsToken != null ? new SecurityTokenElement(actAsToken) : null
            };
        }

        private static Binding GetWindowsWSTrustBinding()
        {
            var binding = new Microsoft.IdentityModel.Protocols.WSTrust.Bindings.WindowsWSTrustBinding(SecurityMode.TransportWithMessageCredential);

            return binding;
        }

        private static Binding GetUserNameWSTrustBinding(SecurityMode securityMode)
        {
            var binding = new WS2007HttpBinding();
            binding.Security.Mode = securityMode;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.NegotiateServiceCredential = true;

            return binding;
        }

        public static void AttachToken(ChannelFactory channel, SecurityToken token)
        {
            channel.Endpoint.Behaviors.Add(new AttachTokenEndpointBehavior(token));
        }
    }
}
