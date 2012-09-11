# shopify.net
===========

Lightweight object-oriented .NET client for the Shopify API

## Author

Colin McDonald - [colinmcdonald.ca](http://colinmcdonald.ca)

## Requirements

* .NET 2.0 or greater
	* the DotNet2.0 folder contains a Visual Studio 2010 solution
	* the DotNet4.5 folder contains a solution created with Visual Studio 11 Beta Express for Web
		*  MVC3 is required to build and run the Sample App in the DotNet4.5

## Installation

For now, the easiest and only, download the source code and add the project to your solution.

## Shopify API Authorization

In order to understand how shopify authorizes your code to make API calls for a certain shopify customer, I recommend reading this document: [Shopify API Authentication](http://api.shopify.com/authentication.html)

### ShopifyAPIAuthorizer

This is the class in this library that will enable your code to quickly authorize your app.

```csharp

    /// <summary>
    /// this class is used to obtain the authorization
    /// from the shopify customer to make api calls on their behalf
    /// </summary>
    public class ShopifyAPIAuthorizer
    {
        /// <summary>
        /// Creates an instance of this class in order to obtain the authorization
        /// from the shopify customer to make api calls on their behalf
        /// </summary>
        /// <param name="shopName">name of the shop to make the calls for.</param>
        /// <param name="apiKey">the unique api key of your app (obtained from the partner area when you create an app).</param>
        /// <param name="secret">the secret associated with your api key.</param>
        /// <remarks>make sure that the shop name parameter is the only the subdomain part of the myshopify.com url.</remarks>
        public ShopifyAPIAuthorizer(string shopName, string apiKey, string secret)

        /// <summary>
        /// Get the URL required by you to redirect the User to in which they will be 
        /// presented with the ability to grant access to your app with the specified scope
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public string GetAuthorizationURL(string[] scope, string redirectUrl = null)

        /// <summary>
        /// After the shop owner has authorized your app, Shopify will give you a code.
        /// Use this code to get your authorization state that you will use to make API calls
        /// </summary>
        /// <param name="code">a code given to you by shopify</param>
        /// <returns>Authorization state needed by the API client to make API calls</returns>
        public ShopifyAuthorizationState AuthorizeClient(string code)
    }

```

### Using ShopifyAPIAuthorizer

This is a quick litte example to show you how you would use the ShopifyAPIAuthorizer class

```csharp

	string shopName = "";// get the shop name from the user (i.e. a web form)
	// you will need to pass a URL that will handle the response from Shopify when it passes you the code parameter
	Uri returnURL = new Uri("http://yourappdomain.com/HandleAuthorization");
	var authorizer = new ShopifyAPIAuthorizer(shopName, 
		ConfigurationManager.AppSettings["Shopify.ConsumerKey"], // In this case I keep my key and secret in my config file
		ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);
	
	// get the Authorization URL and redirect the user
	var authUrl = authorizer.GetAuthorizationURL(new string[] { ConfigurationManager.AppSettings["Shopify.Scope"] }, returnURL.ToString());
	Redirect(authUrl);

	// Meanwhile the User is click "yes" to authorize your app for the specified scope.  
	// Once this click, yes or no, they are redirected back to the return URL

	// Handle the shopify response at the Return URL:

	// get the following variables from the Query String of the request
	string code = "";
	string shop = ""; 
	string error = ""; 

	// check for an error first
	if (!String.IsNullOrEmpty(error))
    {
        this.TempData["Error"] = error;
        return RedirectToAction("Login");
    }

	// make sure we have the code
    if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(shop))
        return RedirectToAction("Index", "Home");

    var shopName = shop.Replace(".myshopify.com", String.Empty);
	var authorizer = new ShopifyAPIAuthorizer(shopName, 
		ConfigurationManager.AppSettings["Shopify.ConsumerKey"], // In this case I keep my key and secret in my config file
		ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);

	// get the authorization state
    ShopifyAuthorizationState authState = authorizer.AuthorizeClient(code);

    if (authState != null && authState.AccessToken != null)
    {
        // store the auth state in the session or DB to be used for all API calls for the specified shop
    }

```

## Shopify API Usage

In order to use the Shopify API you will have to become intimate knowledge-wise with this documentation: [API Docs](http://api.shopify.com/). It is for that reason that I have purposly designed this class.  You will not be hidden from the URLs of the API or the ways in which the API will require the data to be passed.

Once you have used the ShopifyAPIAuthorizer class to get the authorization state you can make API calls.

### Using ShopifyAPIClient

Get all Products from the API.  (.NET 2.0 and up)

```csharp

	ShopifyAPIClient api = new ShopifyAPIClient(authState);

	// by default JSON string is returned
	object data = api.Get("/admin/products.json");

	// use your favorite JSON library to decode the string into a C# object

```

Get all Products from the API. (.NET 4.5)

```csharp

	// pass the supplied JSON Data Translator
	var api = new ShopifyAPIClient(authState, new JsonDataTranslator());

	// The JSON Data Translator will automatically decode the JSON for you
	dynamic data = api.Get("/admin/products.json");

	// the dynamic object will have all the fields just like in the API Docs
	foreach(var product in data.products)
	{
		Console.Write(product.title);
	}
	
```

Create a Product. (.NET 2.0 and up)

```csharp
	
	ShopifyAPIClient api = new ShopifyAPIClient(authState);

	// Manually construct a JSON string or in some other way
	// Ugly
    string dataToUpdate = 
        "{" +
		    "\"product\": {" +
				"\"title\": \"Burton Custom Freestlye 151\"," +
				"\"body_html\": \"<strong>Good snowboard!</strong>\"" +
			"}" +
		"}";

	string createProductResponse = api.Post("/admin/products.json");
	
```

Update a Product. (.NET 4.5)

```csharp

	// pass the supplied JSON Data Translator
	var api = new ShopifyAPIClient(authState, new JsonDataTranslator());
	            
	// use dynamics to create the object
	// a lot nicer that the previous way
	dynamic newProduct = new
    { 
        products = new { 
            title		= "Burton Custom Freestlye 151", 
            body_html	= "<strong>Good snowboard!</strong>"
        } 
    };
    dynamic createProductResponse = api.Post("/admin/products.json", newProduct);

	
```

Delete a Product. (.NET 4.5)

```csharp

	// id of the product you wish to delete
	int id = 123;
	var api = new ShopifyAPIClient(authState, new JsonDataTranslator());
	api.Delete(String.Format("/admin/products/{0}.json", id));

```

## Sample Web Application

This sample application should give you an excellent idea how you will need to perform the required oAuth authentication and API calls.

For the DotNet4.5 folder, first you must install MVC3 and then open, compile and run the web application.  

Once you go through the authorization steps, you can Add/Edit/Delete Shopify Blog objects.

### Web.config for Sample Application

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" connectionString="Data Source=(LocalDb)\v11.0;Initial Catalog=aspnet-SampleWebApp-201278191135;Integrated Security=true" providerName="System.Data.SqlClient" />
  </connectionStrings>
  <appSettings>
    <add key="Shopify.ConsumerKey" value="PUT_API_KEY_HERE"/>
    <add key="Shopify.ConsumerSecret" value="PUT_SECRET_HERE"/>
    <add key="Shopify.Scope" value="write_products,write_content"/><!-- This is just an example scope. -->
  </appSettings>
  <system.web>
    <compilation debug="true" targetFramework="4.5" />
    <httpRuntime targetFramework="4.5" encoderType="System.Web.Security.AntiXss.AntiXssEncoder, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
    <machineKey compatibilityMode="Framework45" />
    <authentication mode="Forms">
      <forms loginUrl="~/Account/Login" timeout="2880" />
    </authentication>
    <pages>
      <namespaces>
        <add namespace="System.Web.Helpers" />
        <add namespace="System.Web.Mvc" />
        <add namespace="System.Web.Mvc.Ajax" />
        <add namespace="System.Web.Mvc.Html" />
        <add namespace="System.Web.Routing" />
        <add namespace="System.Web.WebPages" />
      </namespaces>
    </pages>
    <profile defaultProvider="DefaultProfileProvider">
      <providers>
        <add name="DefaultProfileProvider" type="System.Web.Providers.DefaultProfileProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="DefaultConnection" applicationName="/" />
      </providers>
    </profile>
    <membership defaultProvider="DefaultMembershipProvider">
      <providers>
        <add name="DefaultMembershipProvider" type="System.Web.Providers.DefaultMembershipProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="DefaultConnection" enablePasswordRetrieval="false" enablePasswordReset="true" requiresQuestionAndAnswer="false" requiresUniqueEmail="false" maxInvalidPasswordAttempts="5" minRequiredPasswordLength="6" minRequiredNonalphanumericCharacters="0" passwordAttemptWindow="10" applicationName="/" />
      </providers>
    </membership>
    <roleManager defaultProvider="DefaultRoleProvider">
      <providers>
        <add name="DefaultRoleProvider" type="System.Web.Providers.DefaultRoleProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="DefaultConnection" applicationName="/" />
      </providers>
    </roleManager>
    <sessionState mode="InProc" customProvider="DefaultSessionProvider">
      <providers>
        <add name="DefaultSessionProvider" type="System.Web.Providers.DefaultSessionStateProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="DefaultConnection" applicationName="/" />
      </providers>
    </sessionState>
  </system.web>
  <system.webServer>
    <validation validateIntegratedModeConfiguration="false" />
    <modules runAllManagedModulesForAllRequests="true" />
  </system.webServer>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Helpers" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```