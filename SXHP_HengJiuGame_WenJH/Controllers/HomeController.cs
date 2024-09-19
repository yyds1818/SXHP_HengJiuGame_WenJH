using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class HomeController : BaseLoginController
    {
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson dataJson = new dataTableJson();
        public ActionResult Index()
        {
            if (CurrUser != null)
            {
                ViewBag.User = CurrUser;
            }
            return View();
        }

        public ActionResult Blank()
        {
            return View();
        }

        public ActionResult ModifyPass()
        {
            if (CurrUser != null)
            {
                ViewBag.User = CurrUser;
            }
            return View();
        }

        public ActionResult ModifyInfo()
        {
            if (CurrUser != null)
            {
                ViewBag.User = CurrUser;
            }
            return View();
        }

        public ActionResult QuitLoginBase()
        {
            Session.Remove("User");
            return Content("缓存删除成功");
        }

        public ActionResult ModifyInfoBase(UserInfo user)
        {
            try
            {
                if (CurrUser != null)
                {
                    CurrUser.UserName = user.UserName;
                    CurrUser.sex = user.sex;
                    CurrUser.IDCard = user.IDCard;
                    CurrUser.Email = user.Email;
                    CurrUser.TelPhone = user.TelPhone;
                    CurrUser.Dimission = user.Dimission;
                    CurrUser.Address = user.Address;
                    db.Entry(CurrUser).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        dataJson.code = 0;
                        dataJson.msg = "修改信息成功";
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "修改信息失败";
                    }
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "请先登录账号";
                }

            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "修改信息失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModifyPassBase(UserInfo_EX user)
        {
            try
            {
                if (CurrUser != null)
                {
                    if (user.OldPassWord == CurrUser.PassWord)
                    {
                        CurrUser.PassWord = user.PassWord;
                        db.Entry(CurrUser).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            dataJson.code = 0;
                            dataJson.msg = "修改密码成功";
                        }
                        else
                        {
                            dataJson.code = 1;
                            dataJson.msg = "修改密码失败";
                        }
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "原密码错误，修改失败";
                    }
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "请先登录账号";
                }

            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "修改密码失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }
    }
}