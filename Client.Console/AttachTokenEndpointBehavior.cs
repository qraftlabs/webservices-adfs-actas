using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel;
using System.ServiceModel;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

public class AttachTokenEndpointBehavior : IEndpointBehavior, IClientMessageInspector
    {
        private SecurityToken token;
 
        public AttachTokenEndpointBehavior(SecurityToken token)
        {
            this.token = token;
        }
 
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }
 
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }
 
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }
 
        public void Validate(ServiceEndpoint endpoint)
        {
        }
 
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            try
            {
               reply.Headers.RemoveAll("Security", WSSecurity10Constants.Namespace); // remove the Security header so Exception is not thrown
            }
            catch { };
        }
 
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var wsseHeader = CreateWSSecurityHeader(token);
 
            request.Headers.Add(new SecurityHeader(token));
 
            return null;
        }
 
        private class SecurityHeader : MessageHeader
        {
            SecurityToken token;
 
            public SecurityHeader(SecurityToken token)
            {
                this.token = token;
            }
 
            private XmlElement GetToken()
            {
                XmlElement tokenXml = null;
                if (token is GenericXmlSecurityToken)
                {
                    tokenXml = (token as GenericXmlSecurityToken).TokenXml;
                }
                else
                {
                    var sb = new StringBuilder();
                    var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true });
                    var tokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                    tokenHandlers.WriteToken(writer, token);
 
                    writer.Flush();
                    var doc = new XmlDocument();
                    doc.LoadXml(sb.ToString());
                    tokenXml = doc.DocumentElement;
                }
 
                return tokenXml;
            }
 
            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteRaw(GetToken().OuterXml);
 
                writer.WriteStartElement("Timestamp");
                writer.WriteXmlnsAttribute("", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                writer.WriteAttributeString("Id", "Timestamp-79");
                //Created
                writer.WriteStartElement("Created");
                writer.WriteString(this.token.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                writer.WriteEndElement();
                //Expires
                writer.WriteStartElement("Expires");
                writer.WriteString(this.token.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
 
            public override string Name
            {
                get { return "Security"; }
            }
 
            public override string Namespace
            {
                get { return WSSecurity10Constants.Namespace; }
            }
        }
 
        private static MessageHeader CreateWSSecurityHeader(SecurityToken token)
        {
            XmlElement tokenXml = null;
            if (token is GenericXmlSecurityToken)
            {
                tokenXml = (token as GenericXmlSecurityToken).TokenXml;
            }
            else
            {
                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true });
                var tokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                tokenHandlers.WriteToken(writer, token);
 
                writer.Flush();
                var doc = new XmlDocument();
                doc.LoadXml(sb.ToString());
                tokenXml = doc.DocumentElement;
            }
 
            var wsseHeader = MessageHeader.CreateHeader("Security", WSSecurity10Constants.Namespace, tokenXml, false);
            return wsseHeader;
        }
    }
namespace Client
{
    public class AttachTokenEndpointBehavior : IEndpointBehavior, IClientMessageInspector
    {
        private SecurityToken token;

        public AttachTokenEndpointBehavior(SecurityToken token)
        {
            this.token = token;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            try
            {
                reply.Headers.RemoveAll("Security", WSSecurity10Constants.Namespace); // remove the Security header so Exception is not thrown
            }
            catch { };
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var wsseHeader = CreateWSSecurityHeader(token);

            request.Headers.Add(new SecurityHeader(token));

            return null;
        }

        private class SecurityHeader : MessageHeader
        {
            SecurityToken token;

            public SecurityHeader(SecurityToken token)
            {
                this.token = token;
            }

            private XmlElement GetToken()
            {
                XmlElement tokenXml = null;
                if (token is GenericXmlSecurityToken)
                {
                    tokenXml = (token as GenericXmlSecurityToken).TokenXml;
                }
                else
                {
                    var sb = new StringBuilder();
                    var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true });
                    var tokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                    tokenHandlers.WriteToken(writer, token);

                    writer.Flush();
                    var doc = new XmlDocument();
                    doc.LoadXml(sb.ToString());
                    tokenXml = doc.DocumentElement;
                }

                return tokenXml;
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteRaw(GetToken().OuterXml);

                writer.WriteStartElement("Timestamp");
                writer.WriteXmlnsAttribute("", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                writer.WriteAttributeString("Id", "Timestamp-79");
                //Created
                writer.WriteStartElement("Created");
                writer.WriteString(this.token.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                writer.WriteEndElement();
                //Expires
                writer.WriteStartElement("Expires");
                writer.WriteString(this.token.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            public override string Name
            {
                get { return "Security"; }
            }

            public override string Namespace
            {
                get { return WSSecurity10Constants.Namespace; }
            }
        }

        private static MessageHeader CreateWSSecurityHeader(SecurityToken token)
        {
            XmlElement tokenXml = null;
            if (token is GenericXmlSecurityToken)
            {
                tokenXml = (token as GenericXmlSecurityToken).TokenXml;
            }
            else
            {
                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true });
                var tokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                tokenHandlers.WriteToken(writer, token);

                writer.Flush();
                var doc = new XmlDocument();
                doc.LoadXml(sb.ToString());
                tokenXml = doc.DocumentElement;
            }

            var wsseHeader = MessageHeader.CreateHeader("Security", WSSecurity10Constants.Namespace, tokenXml, false);
            return wsseHeader;
        }
    }
}
