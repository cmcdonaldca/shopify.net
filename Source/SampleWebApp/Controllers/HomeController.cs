using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using SampleWebApp.Shopify;
using ShopifyAPIAdapterLibrary;

namespace SampleWebApp.Controllers
{
    [ShopifyAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Products()
        {
            object shopResponse = _shopify.Get("/admin/products.json");



            return View();

        }

        ShopifyAPIClient _shopify;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ShopifyAuthorizationState authState = ShopifyAuthorize.GetAuthorizationState(this.HttpContext);
            if (authState != null)
            {
                _shopify = new ShopifyAPIClient(authState);
            }
        }
    }
}
