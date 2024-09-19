using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class MenuController : BaseLoginController
    {
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson datajson = new dataTableJson();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取数据和模糊查询
        /// </summary>
        /// <param name="MenuName"></param>
        /// <param name="MenuCode"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetData(string MenuName, string MenuCode, int page, int limit)
        {
            if (string.IsNullOrEmpty(MenuName) && string.IsNullOrEmpty(MenuCode))
            {
                // 查询顶级的数据集合
                var list = db.Menu.Where(e => e.ParentID == null).ToList();
                // 返回的数量应该与查询总数一致
                datajson.count = list.Count;
                // 分页
                var offer = (page - 1) * limit;
                list = list.Skip(offer).Take(limit).ToList();
                // 树形集合
                List<TreeTb_EX> MenuList = new List<TreeTb_EX>();
                foreach (var item in list)
                {
                    TreeTb_EX dataTreeTb = new TreeTb_EX();
                    dataTreeTb.id = item.ID;
                    dataTreeTb.name = item.MenuName;
                    dataTreeTb.code = item.MenuCode;
                    dataTreeTb.type = item.Type;
                    dataTreeTb.url = item.MenuUrl;
                    dataTreeTb.createDate = item.CreateDate;
                    dataTreeTb.parentId = item.ParentID;
                    // 查询是否有子集
                    var searchcount = db.Menu.Where(e => e.ParentID == item.ID).ToList();
                    // 递归
                    dataTreeTb.children = chrildrenSearch(item.ID);
                    // 三元表达式
                    dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                    MenuList.Add(dataTreeTb);
                }
                datajson.code = 0;
                datajson.msg = "数据查询成功";
                datajson.data = MenuList;
            }
            else
            {
                // 模糊查询
                var list = db.Menu.Where(e => e.MenuName.Contains(MenuName) && e.MenuCode.Contains(MenuCode)).ToList();
                // 在分页前获取总数据
                datajson.count = list.Count;
                // 分页
                var offset = (page - 1) * limit;
                list = list.Skip(offset).Take(limit).ToList();
                // 树形集合
                var MenuList = new List<TreeTb_EX>();
                foreach (var item in list)
                {
                    TreeTb_EX dataTreeTb = new TreeTb_EX();
                    dataTreeTb.id = item.ID;
                    dataTreeTb.name = item.MenuName;
                    dataTreeTb.code = item.MenuCode;
                    dataTreeTb.type = item.Type;
                    dataTreeTb.url = item.MenuUrl;
                    dataTreeTb.createDate = item.CreateDate;
                    dataTreeTb.parentId = item.ParentID;
                    // 查询是否有子集
                    var searchcount = db.Menu.Where(e => e.ParentID == item.ID).ToList();
                    // 递归
                    dataTreeTb.children = chrildrenSearch(item.ID);
                    // 三元表达式
                    dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                    MenuList.Add(dataTreeTb);
                }
                datajson.code = 0;
                datajson.msg = "数据查询成功";
                datajson.data = MenuList;
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 递归查询
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IEnumerable<TreeTb_EX> chrildrenSearch(Guid? guid)
        {
            // 查询子集
            var list = db.Menu.Where(e => e.ParentID == guid).ToList();
            var treeJson = new List<TreeTb_EX>();
            foreach (var item in list)
            {
                TreeTb_EX dataTreeTb = new TreeTb_EX();
                dataTreeTb.id = item.ID;
                dataTreeTb.name = item.MenuName;
                dataTreeTb.code = item.MenuCode;
                dataTreeTb.type = item.Type;
                dataTreeTb.url = item.MenuUrl;
                dataTreeTb.createDate = item.CreateDate;
                dataTreeTb.parentId = item.ParentID;
                var searchcount = db.Menu.Where(e => e.ParentID == item.ID).ToList();
                dataTreeTb.children = chrildrenSearch(item.ID);
                dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                treeJson.Add(dataTreeTb);
            }
            return treeJson;
        }

        /// <summary>
        /// 修改时查询上级菜单
        /// </summary>
        /// <param name="Menu"></param>
        /// <returns></returns>
        public ActionResult GetParent(TreeTb_EX Menu)
        {
            var result = db.Menu.Where(e => e.ID == Menu.parentId).ToList();
            if (Menu.parentId == null)
            {
                Menu tb = new Menu();
                tb.MenuName = "已经是顶级菜单了";
                tb.MenuCode = "已经是顶级菜单了";
                tb.Type = 0;
                result.Add(tb);
                datajson.code = 0;
                datajson.msg = "成功";
                datajson.data = result;
            }
            else
            {
                datajson.code = 0;
                datajson.msg = "成功";
                datajson.data = result;
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加顶级菜单
        /// </summary>
        /// <param name="ParentMenuName"></param>
        /// <param name="ParentMenuCode"></param>
        /// <returns></returns>
        public ActionResult AddBase(string ParentMenuName, string ParentMenuCode)
        {
            try
            {
                Menu menu = new Menu();
                menu.ID = Guid.NewGuid();
                menu.ParentID = null;
                menu.MenuName = ParentMenuName;
                menu.MenuCode = ParentMenuCode;
                menu.MenuUrl = "/"+ParentMenuCode;
                menu.Type = 1;
                menu.CreateDate = DateTime.Now;
                db.Menu.Add(menu);
                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "添加顶级菜单成功";
                    datajson.data = menu;
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "添加顶级菜单失败";
                }
            }
            catch (Exception ex)
            {
                datajson.code = 1;
                datajson.msg = "添加顶级菜单失败" + ex.Message;
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加子集和修改方法
        /// </summary>
        /// <param name="MenuInfo"></param>
        /// <returns></returns>
        public ActionResult AddorEditBase(Menu MenuInfo)
        {
            if (MenuInfo.ID == new Guid())
            {
                Menu menu = new Menu();
                menu.ID = Guid.NewGuid();
                menu.ParentID = MenuInfo.ParentID;
                menu.MenuName = MenuInfo.MenuName;
                menu.MenuCode = MenuInfo.MenuCode;
                menu.Type = MenuInfo.Type + 1;
                menu.CreateDate = DateTime.Now;
                db.Menu.Add(menu);
                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "添加子集成功";
                    datajson.data = menu;
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "添加子集失败";
                }
            }
            else
            {
                // 对查询到的数据进行修改
                var list = db.Menu.Find(MenuInfo.ID);
                list.MenuName = MenuInfo.MenuName;
                list.MenuCode = MenuInfo.MenuCode;
                list.MenuUrl = MenuInfo.MenuUrl;
                db.Entry(list).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "修改成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "修改失败";
                }
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public ActionResult DelBase(TreeTb_EX menu)
        {
            try
            {
                var delinfo = db.Menu.FirstOrDefault(e => e.ID == menu.id);
                if (delinfo != null)
                {
                    db.Menu.Remove(delinfo);
                    db.SaveChanges();
                    datajson.code = 0;
                    datajson.msg = "删除成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "删除失败";
                }
            }
            catch (Exception ex)
            {
                datajson.code = 1;
                datajson.msg = "删除失败" + ex.Message;
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }
    }
}