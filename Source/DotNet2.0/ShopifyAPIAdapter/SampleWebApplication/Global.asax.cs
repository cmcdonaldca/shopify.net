using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using ShopifyAPIAdapterLibrary;

namespace SampleWebApplication
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            e.ToString();

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            e.ToString();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (Request.Url.PathAndQuery.Contains("Authorize.aspx"))
                return;

            // ignore non .net files
            if (!Request.FilePath.EndsWith(".aspx"))
                return;

            if (HttpContext.Current.Session == null)
                Response.Redirect("Authorize.aspx");

            // if we are not authenticated, go to Authorize page
            if (HttpContext.Current.Session["Shopify.AuthState"] == null)
                Response.Redirect("Authorize.aspx");

            ShopifyAuthorizationState state = HttpContext.Current.Session["Shopify.AuthState"] as ShopifyAuthorizationState;
            if (state == null)
                Response.Redirect("Authorize.aspx");

            if (String.IsNullOrEmpty(state.AccessToken))
                Response.Redirect("Authorize.aspx");

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}