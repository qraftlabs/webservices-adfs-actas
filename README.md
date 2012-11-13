# What does this sample show?

Shows two clients consuming a service

- A console application, getting a token from ADFS using domain user and password and then calling a web service using that token.
- A web application, logging users with ADFS and then calling the same web service delegating the user identity by asking ADFS a token using ActAs

This is the simplest it can get... well simpler would be to use plain HTTP for all the interactions, but this is good enough taking into account that we are talking about monsters like WCF, Ws-Trust, SAML, etc.

## Let the code speaks for itself

### Client

* [Getting a token using user/password](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Client.Console/Program.cs#L22)
* [Attaching the token to the SOAP message](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Client.Console/Program.cs#L29)
* [The corresponding behavior that will attach the token](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Client.Console/AttachTokenEndpointBehavior.cs)
* [Client side config, simple basicHttpBinding](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Client.Console/app.config)

### Service

* [Configuring WIF to validate tokens](https://github.com/qraftlabs/webservices-adfs-actasblob/master/Services.Console/app.config#L30)
* [Adding a ServiceAuthorizationManager that validates token to a basichttpbinding service](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Services.Console/app.config#L11)
* [Validating the token](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Services.Console/ValidateSamlToken.cs#L19)
* [Consuming the claims from the principal](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/Services.Console/SampleService.cs#L19)

### WebClient

* [Getting a token delegating the identity of the logged in user](https://github.com/qraftlabs/webservices-adfs-actas/blob/master/WebClient/Controllers/HomeController.cs#L17)

**IMPORTANT**: this is using bearer tokens, so you have to run the service on SSL to be safe. The sample is using http and self host, don't do this in production :)
 