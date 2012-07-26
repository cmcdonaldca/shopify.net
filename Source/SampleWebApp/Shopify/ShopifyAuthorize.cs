using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopifyAPIAdapterLibrary;

namespace SampleWebApp.Shopify
{
    public class ShopifyAuthorize : AuthorizeAttribute
    {
        private static readonly string AuthSessionKey = "shopify_auth_state";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionState"></param>
        /// <param name="state"></param>
        public static void SetAuthorization(System.Web.HttpContextBase httpContext, ShopifyAuthorizationState state)
        {
            httpContext.Session[AuthSessionKey] = state;
        }

        /// <summary>
        /// Test to see if the current http context is authorized for access to Shopify API
        /// </summary>
        /// <param name="httpContext">current httpContext</param>
        /// <returns>true if the current http context is authorized for access to Shopify API, otherwise false</returns>
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var authState = GetAuthorizationState(httpContext);
            if (authState == null || String.IsNullOrWhiteSpace(authState.AccessToken))
                return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static ShopifyAuthorizationState GetAuthorizationState(System.Web.HttpContextBase httpContext)
        {
            return httpContext.Session[AuthSessionKey] as ShopifyAuthorizationState;
        }
    }
}