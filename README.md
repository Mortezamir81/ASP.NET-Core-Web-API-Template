# ASP.NET-Core-Web-API-Template

`ASP.NET-Core-Web-API-Template` A light, clean and fast template for ASP.NET Core Web API Developed with &hearts; by Morteza Mirshkar.

## Features
- Implemented by .NET 7
- Architecture: N-Layer
- Support Versioning
- Support Anti Dos
- Support Anti XSS
- Global Exception Handling
- Support AutoMapper
- Support caching by EasyCaching
- Support NLog For logging
- Full and rich Swagger support
	- Authentication With OAuth2 (Username, Password).
	- Description of controllers and actions by c# XML docs (Auto or Manual).
	- Explanations about the features of view models.
	- Response time for responses with status code 200.
	- Support for displaying titles of enums.
- Support Auto Register Services
	- By inheriting from (IRegisterAsTransient or IRegisterAsScoped or IRegisterAsSingleton) by services
- Authentication and Authorization with Identity
	- Authorization with JWT and Refresh Token
	- Authentication by OAuth2 (for swagger)



## Config Connection String
### In Development
You can fill `ConnectionStrings:SqlConnectionString` section in `appsettings.Development.json`.

### In Production
You can set enviroment variable with key `ConnectionStrings:SqlConnectionString` by `Web.config` or other way!


