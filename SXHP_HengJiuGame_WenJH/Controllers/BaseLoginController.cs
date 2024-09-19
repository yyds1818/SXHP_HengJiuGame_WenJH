using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    [LoginFilterController]
    /// <summary>
    /// 专门取session的值的类
    /// </summary>
    public class BaseLoginController : Controller
    {
        // GET: BaseLogin
        public static UserInfo CurrUser = null;
        /// <summary>
        /// 构造函数 
        /// </summary>
        public BaseLoginController() { 
            var curr = System.Web.HttpContext.Current.Session["User"];
            if (curr != null)
            {
                // 拆箱
                CurrUser = curr as UserInfo;
            }
        }
    }
}