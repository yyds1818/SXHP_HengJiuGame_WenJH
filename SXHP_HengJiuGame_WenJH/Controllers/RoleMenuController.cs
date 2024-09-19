using NPOI.POIFS.Crypt;
using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class RoleMenuController : BaseLoginController
    {
        // GET: RoleMenu
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson dataJson = new dataTableJson();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 展示列表和查询功能
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="JobName"></param>
        /// <param name="JobCode"></param>
        /// <returns></returns>
        public ActionResult GetData(int page, int limit, string rolename, string rolecode)
        {
            try
            {
                //三表join linq写法
                var queru = (from u in db.Roles
                             join p in db.RoleMenus on u.ID equals p.RoleID
                             into roleMenus
                             from r in roleMenus.DefaultIfEmpty() // 对RoleMenus进行左连接  
                             join s in db.Menu on r.MenuID equals s.ID
                             into menus
                             from m in menus.DefaultIfEmpty() // 对Menu进行左连接  
                             group new { u.ID, u.RoleName, u.RoleCode, u.CreateDate, m.MenuName } by u.ID into g
                             select new
                             {
                                 ID = g.Key,
                                 RoleName = g.Select(x => x.RoleName).FirstOrDefault(),
                                 RoleCode = g.Select(x => x.RoleCode).FirstOrDefault(),
                                 CreateDate = g.Select(x => x.CreateDate).FirstOrDefault(),
                                 MenuNames = g.Select(x => x.MenuName).Where(name => name != null).ToList() // 过滤掉null的MenuName  
                             }).ToList();
                if (string.IsNullOrEmpty(rolename) && string.IsNullOrEmpty(rolecode))
                {
                    var offset = (page - 1) * limit;
                    dataJson.count = queru.Count;
                    dataJson.code = 0;
                    dataJson.msg = "查询成功";
                    dataJson.data = queru.Skip(offset).Take(limit).ToList();
                }
                else
                {
                    var list = db.Roles.Where(e => e.RoleName.Contains(rolename) && e.RoleCode.Contains(rolecode)).ToList();
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

        /// <summary>
        /// 添加和修改功能
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public ActionResult AddorEditBase(Roles role)
        {
            try
            {
                if (CurrUser != null)
                {
                    if (role.ID == new Guid())
                    {
                        // 添加功能
                        role.ID = Guid.NewGuid();
                        role.CreateDate = DateTime.Now;
                        db.Roles.Add(role);
                    }
                    else
                    {
                        // 修改功能
                        var user = db.Roles.FirstOrDefault(e => e.ID == role.ID);
                        user.RoleName = role.RoleName;
                        user.RoleCode = role.RoleCode;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        dataJson.code = 0;
                        dataJson.msg = "保存成功";
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "保存失败";
                    }
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "请先进行登录";
                }
            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "确认失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除功能
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult DelBase(Guid? ID)
        {
            try
            {
                var dellist = db.Roles.Where(x => x.ID == ID).FirstOrDefault();
                if (dellist != null)
                {
                    db.Roles.Remove(dellist);
                    if (db.SaveChanges() > 0)
                    {
                        dataJson.code = 0;
                        dataJson.msg = "删除成功";
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "删除失败";
                    }
                }
                else
                {
                    dataJson.code = 1;
                    dataJson.msg = "删除失败，未找到对应数据";
                }
            }
            catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "删除失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddRelation(List<TreeTb_EX> menus,Guid role)
        {
            try
            {
                #region 清空选中的所有菜单
                var list = db.RoleMenus.Where(x => x.RoleID == role).ToList();
                if (list != null)
                {
                    db.RoleMenus.RemoveRange(list);
                }
                #endregion

                #region 添加新菜单
                foreach (var menu in menus)
                {
                    RoleMenus roleMenus = new RoleMenus();
                    roleMenus.ID = Guid.NewGuid();
                    roleMenus.RoleID = role;
                    roleMenus.MenuID = menu.id;
                    db.RoleMenus.Add(roleMenus);
                }
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
            }catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "设置失败：" + ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }
    }
}