using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class UserRoleController : BaseLoginController
    {
        // GET: UserRole
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson dataJson = new dataTableJson();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(int page, int limit, string name, string code)
        {
            try
            {
                //三表join linq写法
                var queru = (from u in db.UserInfo
                            join p in db.UserRole on u.ID equals p.UserID
                            into d
                            from r in d.DefaultIfEmpty()

                            join s in db.Roles on r.RoleID equals s.ID
                            into roles
                            from i in roles.DefaultIfEmpty()
                            select new
                            {
                                ID = u.ID,
                                RoleID = r.RoleID,
                                UserName = u.UserName,
                                UserCode = u.UserCode,
                                CreateDate = u.CreateDate,
                                RoleName = i.RoleName,
                            }).ToList();
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(code))
                {
                    var offset = (page - 1) * limit;
                    dataJson.count = queru.Count;
                    dataJson.code = 0;
                    dataJson.msg = "查询成功";
                    dataJson.data = queru.Skip(offset).Take(limit).ToList();
                }
                else
                {
                    var list = db.UserInfo.Where(e => e.UserName.Contains(name) && e.UserCode.Contains(code)).ToList();
                    var offset = (page - 1) * limit;
                    if (list.Count > 0)
                    {
                        dataJson.code = 0;
                        dataJson.count = list.Count;
                        dataJson.msg = "查询成功";
                        dataJson.data = list.Skip(offset).Take(limit).ToList();
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "查询失败";
                    }
                }
            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "查询失败:" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddRelation(Guid? rid, Guid? uid)
        {
            try
            {
                #region 清空选中的所有菜单
                var list = db.UserRole.Where(x => x.UserID == uid).ToList();
                if (list != null)
                {
                    db.UserRole.RemoveRange(list);
                }
                #endregion

                #region 添加新菜单
                UserRole userRel = new UserRole();
                userRel.ID = Guid.NewGuid();
                userRel.UserID = uid;
                userRel.RoleID = rid;
                db.UserRole.Add(userRel);
                #endregion

                if (db.SaveChanges() > 0)
                {
                    dataJson.code = 0;
                    dataJson.msg = "设置成功";
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "设置失败";
                }
            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "设置失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }
    }
}