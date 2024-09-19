using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class LoginController : Controller
    {
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson dataJson = new dataTableJson();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Registe()
        {
            return View();
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult LoginBase(UserInfo user)
        {
            // 判断用户输入是否为空
            if (!string.IsNullOrEmpty(user.UserCode) && !string.IsNullOrEmpty(user.PassWord))
            {
                // 查询表数据
                var entity = db.UserInfo.Where(e => e.UserCode == user.UserCode && e.PassWord == user.PassWord).FirstOrDefault();
                // 判断是否查询到数据
                if (entity != null)
                {
                    // 装箱 将登录的用户信息保存在session中
                    Session["User"] = entity;
                    dataJson.code = 0;
                    dataJson.data = entity;
                    dataJson.msg = "登录成功";
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "账号或密码错误";
                }
            }
            else
            {
                dataJson.code = 1;
                dataJson.msg = "账号或密码不能为空";
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult RegisteBase(UserInfo user)
        {
            #region 用户注册 对用户表添加新数据
            try
            {
                user.ID = Guid.NewGuid();
                user.CreateDate = DateTime.Now;
                var back = db.UserInfo.Add(user);
                if (db.SaveChanges() > 0)
                {
                    dataJson.code = 0;
                    dataJson.msg = "注册成功";
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "注册失败";
                }
            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "注册失败" + ex.Message;
            }
            #endregion

            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }
    }
}