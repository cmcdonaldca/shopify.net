using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShopifyAPIAdapterLibrary;

namespace SampleWebApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShopifyAuthorizationState state = HttpContext.Current.Session["Shopify.AuthState"] as ShopifyAuthorizationState;
            ShopifyAPIClient client
                = new ShopifyAPIClient(state);
            APIOutput.Text = (string)client.Get("/admin/products.json");
        }
    }
}