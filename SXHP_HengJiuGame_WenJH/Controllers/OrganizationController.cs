using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class OrganizationController : BaseLoginController
    {
        // GET: Organization
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson datajson = new dataTableJson();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取所需数据和模糊查询
        /// </summary>
        /// <param name="StructureName"></param>
        /// <param name="StructureCode"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetData(string StructureName, string StructureCode, int page, int limit)
        {
            #region 查询
            if (string.IsNullOrEmpty(StructureCode) && string.IsNullOrEmpty(StructureName))
            {
                // 查询顶级的数据集合
                var list = db.Structure.Where(e => e.ParentID == null).OrderBy(e => e.Type).ToList();
                // 返回的数量应该与查询总数一致
                datajson.count = list.Count;
                // 分页
                var offer = (page - 1) * limit;
                list = list.Skip(offer).Take(limit).ToList();
                // 树形集合
                List<TreeTb_EX> StruList = new List<TreeTb_EX>();
                foreach (var item in list)
                {
                    TreeTb_EX dataTreeTb = new TreeTb_EX();
                    dataTreeTb.id = item.ID;
                    dataTreeTb.name = item.StructureName;
                    dataTreeTb.code = item.StructureCode;
                    dataTreeTb.type = item.Type;
                    dataTreeTb.createDate = item.CreateDate;
                    dataTreeTb.parentId = item.ParentID;
                    // 查询是否有子集
                    var searchcount = db.Structure.Where(e => e.ParentID == item.ID).ToList();
                    // 递归
                    dataTreeTb.children = chrildrenSearch(item.ID);
                    // 三元表达式
                    dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                    StruList.Add(dataTreeTb);
                }
                datajson.code = 0;
                datajson.msg = "数据查询成功";
                datajson.data = StruList;
            }
            else
            {
                // 模糊查询
                var list = db.Structure.Where(e => e.StructureName.Contains(StructureName) && e.StructureCode.Contains(StructureCode)).ToList();
                // 在分页前获取总数据
                datajson.count = list.Count;
                // 分页
                var offset = (page - 1) * limit;
                list = list.Skip(offset).Take(limit).ToList();
                // 树形集合
                var StruList = new List<TreeTb_EX>();
                foreach (var item in list)
                {
                    TreeTb_EX dataTreeTb = new TreeTb_EX();
                    dataTreeTb.id = item.ID;
                    dataTreeTb.name = item.StructureName;
                    dataTreeTb.code = item.StructureCode;
                    dataTreeTb.type = item.Type;
                    dataTreeTb.createDate = item.CreateDate;
                    dataTreeTb.parentId = item.ParentID;
                    // 查询是否有子集
                    var searchcount = db.Structure.Where(e => e.ParentID == item.ID).ToList();
                    // 递归
                    dataTreeTb.children = chrildrenSearch(item.ID);
                    // 三元表达式
                    dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                    StruList.Add(dataTreeTb);
                }
                datajson.code = 0;
                datajson.msg = "数据查询成功";
                datajson.data = StruList;
            }
            #endregion
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 树形结构的方法
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IEnumerable<TreeTb_EX> chrildrenSearch(Guid? guid)
        {
            #region 递归方法
            // 获取符合条件的ID
            var list = db.Structure.Where(e => e.ParentID == guid).ToList();
            // 树形结构
            List<TreeTb_EX> treeJson = new List<TreeTb_EX>();
            foreach (var item in list)
            {
                TreeTb_EX dataTreeTb = new TreeTb_EX();
                dataTreeTb.id = item.ID;
                dataTreeTb.name = item.StructureName;
                dataTreeTb.code = item.StructureCode;
                dataTreeTb.type = item.Type;
                dataTreeTb.createDate = DateTime.Now;
                dataTreeTb.parentId = item.ParentID;
                // 查询是否有子集
                var searchcount = db.Structure.Where(e => e.ParentID == item.ID).ToList();
                // 递归
                dataTreeTb.children = chrildrenSearch(item.ID);
                // 三元表达式
                dataTreeTb.isParent = searchcount.Count > 0 ? true : false;
                treeJson.Add(dataTreeTb);
            }
            #endregion
            return treeJson;
        }

        /// <summary>
        /// 添加顶级机构
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public ActionResult TopAddBase(Structure structure)
        {
            #region 顶级机构的添加方法
            try
            {
                structure.ID = Guid.NewGuid();
                structure.ParentID = null;
                structure.Type = 1;
                structure.CreateDate = DateTime.Now;
                db.Structure.Add(structure);
                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "添加顶级机构成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "添加顶级机构失败";
                }
            }
            catch (Exception ex)
            {
                datajson.code = 1;
                datajson.msg = "添加顶级机构失败：" + ex.Message;
            }
            #endregion
            return Json(datajson,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改和添加方法
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public ActionResult AddorEditBase(Structure structure)
        {
            #region 添加和修改方法
            try
            {
                // id为空时，执行添加方法
                if (structure.ID == Guid.Empty)
                {
                    structure.ID = Guid.NewGuid();
                    structure.Type += 1;
                    structure.CreateDate = DateTime.Now;
                    db.Structure.Add(structure);
                }
                // id有值时，执行修改方法
                else
                {
                    var list = db.Structure.Where(e => e.ID == structure.ID).FirstOrDefault();
                    list.StructureName = structure.StructureName;
                    list.StructureCode = structure.StructureCode;
                    db.Entry(list).State = System.Data.Entity.EntityState.Modified;
                }

                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "确认成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "确认失败";
                }
            }
            catch (Exception ex)
            {
                datajson.code = 1;
                datajson.msg = "确认失败：" + ex.Message;
            }
            #endregion
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询父级
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public ActionResult GetParent(Structure structure)
        {
            #region 获取当前父级
            Structure NewOrg = new Structure();
            if (structure.ParentID == null)
            {
                NewOrg.StructureName = "已经是顶级机构了";
                NewOrg.StructureCode = "已经是顶级机构了";
                NewOrg.Type = 0;
            }
            else
            {
                NewOrg = db.Structure.Where(e => e.ID == structure.ParentID).FirstOrDefault();
            }
            #endregion
            return Json(NewOrg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public ActionResult DelBase(Structure structure)
        {
            #region 删除方法
            try
            {
                var list = db.Structure.Where(e => e.ID == structure.ID).FirstOrDefault();
                db.Structure.Remove(list);
                if (db.SaveChanges() > 0)
                {
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
                datajson.msg = "删除失败：" + ex.Message;
            }
            #endregion
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }
    }
}