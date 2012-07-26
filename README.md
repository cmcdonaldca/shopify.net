# shopify.net
===========

Lightweight object-oriented .NET client for the Shopify API

## Author

	Colin McDonald - [colinmcdonald.ca](http://colinmcdonald.ca)

## Requirements

	* .NET 2.0 or greater
	*  MVC3 to build and run the Sample App

	NOTE: Currently if you are using a version of .NET less that 4.5 you will have to create a project and add the Shopify classed to it manually.  But you will have no major code changes to perform.

## Installation

	* For now, the easiest and only, download the source code and add the project to your solution.

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