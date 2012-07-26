using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// this class is used to obtain the authorization
    /// from the shopify customer to make api calls on their behalf
    /// </summary>
    public class ShopifyAPIAuthorizer
    {
        private string _apiKey;
        private string _secret;
        private string _shopName;

        /// <summary>
        /// Creates an instance of this class in order to obtain the authorization
        /// from the shopify customer to make api calls on their behalf
        /// </summary>
        /// <param name="shopName">name of the shop to make the calls for.</param>
        /// <param name="apiKey">the unique api key of your app (obtained from the partner area when you create an app).</param>
        /// <param name="secret">the secret associated with your api key.</param>
        /// <remarks>make sure that the shop name parameter is the only the subdomain part of the myshopify.com url.</remarks>
        public ShopifyAPIAuthorizer(string shopName, string apiKey, string secret)
        {
            if (shopName == null)
                throw new ArgumentNullException("shopName");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey", "Make sure you have this in your config file.");
            if (secret == null)
                throw new ArgumentNullException("secret", "Make sure you have this in your config file.");

            if (shopName.Length == 0)
                throw new ArgumentException("Can't be an empty string.", "shopName");
            if (apiKey.Length == 0)
                throw new ArgumentException("Make sure you have this in your config file.", "apiKey");
            if (secret.Length == 0)
                throw new ArgumentException("Make sure you have this in your config file.", "secret");

            if (shopName.Contains("."))
                throw new ArgumentException("make sure that the shop name parameter is the only the subdomain part of the myshopify.com url.", "shopName");

            this._shopName = shopName;
            this._apiKey = apiKey;
            this._secret = secret;
        }

        /// <summary>
        /// Get the URL required by you to redirect the User to in which they will be 
        /// presented with the ability to grant access to your app with the specified scope
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public string GetAuthorizationURL(string[] scope, string redirectUrl = null)
        {
            var authURL = new StringBuilder();

            authURL.AppendFormat("http://{0}.myshopify.com/admin/oauth/authorize", this._shopName);
            authURL.AppendFormat("?client_id={0}", this._apiKey);

            if (scope != null && scope.Length > 0)
            {
                string commaSeperatedScope = String.Join(",", scope);
                if (!String.IsNullOrEmpty(commaSeperatedScope))
                    authURL.AppendFormat("&scope={0}", HttpUtility.UrlEncode(commaSeperatedScope));
            }

            if (redirectUrl != null && redirectUrl.Length > 0)
            {
                authURL.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(redirectUrl));
            }

            return authURL.ToString();
        }

        /// <summary>
        /// After the shop owner has authorized your app, Shopify will give you a code.
        /// Use this code to get your authorization state that you will use to make API calls
        /// </summary>
        /// <param name="code">a code given to you by shopify</param>
        /// <returns>Authorization state needed by the API client to make API calls</returns>
        public ShopifyAuthorizationState AuthorizeClient(string code)
        {
            string url = String.Format("https://{0}.myshopify.com/admin/oauth/access_token", _shopName);
            string postBody = String.Format("client_id={0}&client_secret={1}&code={2}",
                _apiKey,    // {0}
                _secret,    // {1}
                code);      // {2}

            HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(url);
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded";
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(authRequest.GetRequestStream()))
                {
                    writer.Write(postBody);
                    writer.Close();
                }
            }

            var response = (HttpWebResponse)authRequest.GetResponse();
            string result = null;

            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }

            if (!String.IsNullOrEmpty(result))
            {
                // it's JSON so decode it
                JObject jsonResult = JObject.Parse(result);
                return new ShopifyAuthorizationState
                {
                    ShopName = this._shopName,
                    AccessToken = (string)jsonResult["access_token"]
                };
            }

            return null;
        }
    }

}
