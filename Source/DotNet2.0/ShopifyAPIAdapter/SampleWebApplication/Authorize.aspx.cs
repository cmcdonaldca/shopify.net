using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using ShopifyAPIAdapterLibrary;

namespace SampleWebApplication
{
    public partial class Authorize : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // check if there is a code or error or shop name in the query  string
                if (!String.IsNullOrEmpty(Request.QueryString["code"]))
                {
                    string shopName = Request.QueryString["shop"].Replace(".myshopify.com", String.Empty);

                    var authorizer = new ShopifyAPIAuthorizer(shopName,
                        ConfigurationManager.AppSettings["Shopify.ConsumerKey"], // In this case I keep my key and secret in my config file
                        ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);

                    Session["Shopify.AuthState"] = authorizer.AuthorizeClient(Request.QueryString["code"]);
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void GetAuthorization_Click(Object sender, EventArgs e)
        {
            string shopName = this.ShopName.Text;// get the shop name from the user (i.e. a web form)

            // you will need to pass a URL that will handle the response from Shopify when it passes you the code parameter
            Uri returnURL = new Uri(Request.Url.ToString());
            var authorizer = new ShopifyAPIAuthorizer(shopName,
                ConfigurationManager.AppSettings["Shopify.ConsumerKey"], // In this case I keep my key and secret in my config file
                ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);

            // get the Authorization URL and redirect the user
            var authUrl = authorizer.GetAuthorizationURL(new string[] { ConfigurationManager.AppSettings["Shopify.Scope"] }, returnURL.ToString());
            Response.Redirect(authUrl);
        }
    }
}