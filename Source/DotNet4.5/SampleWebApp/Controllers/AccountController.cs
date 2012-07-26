using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SampleWebApp.Models;
using ShopifyAPIAdapterLibrary;

namespace SampleWebApp.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// show the login form
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// posted from the login form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // strip the .myshopify.com in case they added it
                string shopName = model.ShopName.Replace(".myshopify.com", String.Empty);

                // prepare the URL that will be executed after authorization is requested
                Uri requestUrl = this.Url.RequestContext.HttpContext.Request.Url;
                Uri returnURL = new Uri(string.Format("{0}://{1}{2}",
                                                        requestUrl.Scheme,
                                                        requestUrl.Authority,
                                                        this.Url.Action("ShopifyAuthCallback", "Account")));

                var authorizer = new ShopifyAPIAuthorizer(shopName, ConfigurationManager.AppSettings["Shopify.ConsumerKey"], ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);
                var authUrl = authorizer.GetAuthorizationURL(new string[] { ConfigurationManager.AppSettings["Shopify.Scope"] }, returnURL.ToString());
                return Redirect(authUrl);
            }

            return View(model);
        }

        /// <summary>
        /// This action is called by shopify after the authorization has finished
        /// </summary>
        /// <param name="code"></param>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShopifyAuthCallback(string code, string shop, string error)
        {
            if (!String.IsNullOrEmpty(error))
            {
                this.TempData["Error"] = error;
                return RedirectToAction("Login");
            }
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(shop))
                return RedirectToAction("Index", "Home");

            var shopName = shop.Replace(".myshopify.com", String.Empty);
            var authorizer = new ShopifyAPIAuthorizer(shopName, ConfigurationManager.AppSettings["Shopify.ConsumerKey"], ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);

            ShopifyAuthorizationState authState = authorizer.AuthorizeClient(code);
            if (authState != null && authState.AccessToken != null)
            {
                Shopify.ShopifyAuthorize.SetAuthorization(this.HttpContext, authState);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
