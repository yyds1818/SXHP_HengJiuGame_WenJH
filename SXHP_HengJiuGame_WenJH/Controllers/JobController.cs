using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class JobController : BaseLoginController
    {
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson dataJson = new dataTableJson();
        // GET: Job
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
        public ActionResult GetData(int page,int limit,string JobName,string JobCode)
        {
            try
            {
                var list = db.Job.ToList();
                var offset = (page - 1) * limit;
                list = list.Skip(offset).Take(limit).ToList();
                if (string.IsNullOrEmpty(JobName) && string.IsNullOrEmpty(JobCode))
                {
                    dataJson.code = 0;
                    dataJson.count = list.Count;
                    dataJson.msg = "查询成功";
                    dataJson.data = list;
                }
                else
                {
                    list = db.Job.Where(e => e.JobName.Contains(JobName) && e.JobCode.Contains(JobCode)).ToList();
                    list = list.Skip(offset).Take(limit).ToList();
                    if (list.Count > 0)
                    {
                        dataJson.code = 0;
                        dataJson.count = list.Count;
                        dataJson.msg = "查询成功";
                        dataJson.data = list;
                    }
                    else
                    {
                        dataJson.code = 1;
                        dataJson.msg = "查询失败";
                    }
                }
            }catch (Exception ex)
            {
                dataJson.code = 1;
                dataJson.msg = "查询失败:"+ex.Message;
            }
            return Json(dataJson,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加和修改功能
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public ActionResult AddorEditBase(Job job)
        {
            try
            {
                if(CurrUser != null)
                {
                    Job user = new Job();
                    if (job.ID == new Guid())
                    {
                        // 添加功能
                        user.ID= Guid.NewGuid();
                        user.JobCode = job.JobCode;
                        user.JobName = job.JobName;
                        user.CreateDate = DateTime.Now;
                        user.CreateUser = CurrUser.UserName;
                        user.ModIfyDate = DateTime.Now;
                        user.ModifyUser = CurrUser.UserName;
                        db.Job.Add(user);
                    }
                    else
                    {
                        // 修改功能
                        user.ID = job.ID;
                        user.JobCode = job.JobCode;
                        user.JobName = job.JobName;
                        user.ModIfyDate = DateTime.Now;
                        user.ModifyUser = CurrUser.UserName;
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
                    dataJson.msg = "请先进行登录" ;
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
                var dellist = db.Job.Where(x => x.ID == ID).FirstOrDefault();
                if(dellist != null)
                {
                    db.Job.Remove(dellist);
                    if(db.SaveChanges() > 0)
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
                dataJson.msg = "删除失败："+ex.Message;
            }
            return Json(dataJson, JsonRequestBehavior.AllowGet);
        }
    }
}