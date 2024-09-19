using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class LoginFilterController : ActionFilterAttribute
    {
        // GET: LoginFilter
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(System.Web.HttpContext.Current.Session["User"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
        }
    }
}