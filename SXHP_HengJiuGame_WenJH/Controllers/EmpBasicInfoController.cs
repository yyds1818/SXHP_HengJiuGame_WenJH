using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SXHP_HengJiuGame_WenJH.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SXHP_HengJiuGame_WenJH.Controllers
{
    public class EmpBasicInfoController : BaseLoginController
    {
        HenJiuGameEntities db = new HenJiuGameEntities();
        dataTableJson datajson = new dataTableJson();
        // GET: EmpBasicInfo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddorEditPage()
        {
            return View();
        }

        public ActionResult GetData(int page,int limit,string EmpBasicInfoCode, bool? Dimission)
        {
            var query = (from user in db.UserInfo
                         join job in db.Job on user.JobID equals job.ID
                         into c
                         from d in c.DefaultIfEmpty()

                         join org in db.Structure on user.StructureID equals org.ID
                         into e
                         from f in e.DefaultIfEmpty()
                         select new
                         {
                             ID = user.ID,
                             UserCode = user.UserCode,
                             UserName = user.UserName,
                             sex = user.sex,
                             Native = user.Native,
                             IDCard = user.IDCard,
                             Address = user.Address,
                             Email = user.Email,
                             TelPhone = user.TelPhone,
                             Dimission = user.Dimission,
                             CreateDate = user.CreateDate,
                             job = d.JobName,
                             structure = f.StructureName
                         }).ToList();
            if (!string.IsNullOrEmpty(EmpBasicInfoCode))
            {
                query = query.Where(e => e.UserCode.Contains(EmpBasicInfoCode)).ToList();
            }
            if(Dimission != null)
            {
                query = query.Where(e => e.Dimission == Dimission).ToList();
            }
            var offset = (page - 1) * limit;
            datajson.code = 0;
            datajson.msg = "查询成功";
            datajson.count = query.Count();
            datajson.data = query.Skip(offset).Take(limit).ToList();

            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddorEditBase(UserInfo_EX user)
        {
            // 添加
            if (user.ID == new Guid())
            {
                UserInfo userinfo = new UserInfo();
                userinfo.ID = Guid.NewGuid();
                userinfo.UserCode = user.UserCode;
                userinfo.UserName = user.UserName;
                // 默认密码
                userinfo.PassWord = "123";
                userinfo.sex = user.sex;
                userinfo.IDCard = user.IDCard;
                userinfo.Email = user.Email;

                userinfo.TelPhone = user.TelPhone;
                userinfo.Dimission = user.Dimission;
                userinfo.Native = user.Native;
                userinfo.Address = user.Address;
                var joblist = db.Job.FirstOrDefault(e => e.JobName == user.job);
                userinfo.JobID = joblist.ID;
                Structure structurelist = db.Structure.FirstOrDefault(e => e.StructureName == user.structure);
                userinfo.StructureID = structurelist.ID;
                userinfo.CreateDate = DateTime.Now;
                db.UserInfo.Add(userinfo);
                if (db.SaveChanges() > 0)
                {
                    datajson.code = 0;
                    datajson.msg = "添加成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "添加失败";
                }
            }
            //修改
            else
            {
                var userlist = db.UserInfo.FirstOrDefault(e => e.ID == user.ID);
                userlist.UserCode = user.UserCode;
                userlist.UserName = user.UserName;
                userlist.Native = user.Native;
                userlist.sex = user.sex;
                userlist.IDCard = user.IDCard;
                userlist.Email = user.Email;
                userlist.TelPhone = user.TelPhone;
                userlist.Dimission = user.Dimission;
                userlist.Address = user.Address;

                var joblist = db.Job.FirstOrDefault(e => e.JobName == user.job);
                userlist.JobID = joblist.ID;
                var structurelist = db.Structure.FirstOrDefault(e => e.StructureName == user.structure);
                userlist.StructureID = structurelist.ID;
                db.Entry(userlist).State = EntityState.Modified;
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
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult DelBase(Guid? ID)
        {
            try
            {
                if (ID != null)
                {
                    var Dellist = db.UserInfo.FirstOrDefault(e => e.ID == ID);
                    db.UserInfo.Remove(Dellist); 
                    db.SaveChanges();
                    datajson.code = 0;
                    datajson.msg = "删除成功";
                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "选中数据不存在";
                }
            }catch (Exception ex)
            {
                datajson.code = 1;
                datajson.msg = "删除失败："+ex.Message;
            }
            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        // 导入
        public ActionResult DataImport(HttpPostedFileBase file)
        {
            //判断是否提交excel文件
            var FileName = file.FileName.Split('.');
            if (file != null && file.ContentLength > 0)
            {
                if (FileName[1] == "xls" || FileName[1] == "xlsx")
                {
                    //首先我们需要导入数据的话第一步其实就是先把excel数据保存到本地中，然后通过Npoi封装的方法去读取已保存的Excel数据
                    string DictorysPath = Server.MapPath("~/Content/ExcelFiles/" + DateTime.Now.ToString("yyyyMMdd"));
                    if (!System.IO.Directory.Exists(DictorysPath))
                    {
                        System.IO.Directory.CreateDirectory(DictorysPath);
                    }

                    file.SaveAs(System.IO.Path.Combine(DictorysPath, file.FileName));

                    //将Excel数据转化为DataTable数据源
                    DataTable Dt = NpoiHelper.Import(System.IO.Path.Combine(DictorysPath, file.FileName), FileName[1]);
                    List<UserInfo> list = new List<UserInfo>();
                    datajson.count = Dt.Rows.Count;
                    List<ImportMsg> importData = new List<ImportMsg>();

                    for (int i = 0; i < Dt.Rows.Count; i++)
                    {
                        ImportMsg info = new ImportMsg();
                        info.RowNumber = i + 1;
                        UserInfo U = new UserInfo();
                        try
                        {
                            U.ID = Guid.NewGuid();
                            U.CreateDate = DateTime.Now;
                            U.PassWord = "123";
                            U.UserName = Dt.Rows[i][0].ToString();
                            U.UserCode = Dt.Rows[i][1].ToString();
                            U.sex = Dt.Rows[i][2].ToString() == "男" ? false : true;
                            U.IDCard = Dt.Rows[i][3].ToString();
                            U.Email = Dt.Rows[i][4].ToString();
                            U.TelPhone = Dt.Rows[i][5].ToString();
                            U.Dimission = Dt.Rows[i][6].ToString() == "是" ? true : false;
                            U.Native = Dt.Rows[i][7].ToString();
                            U.Address = Dt.Rows[i][8].ToString();

                            var jobname = Dt.Rows[i][9].ToString();
                            var jobinfo = db.Job.Where(e => e.JobName == jobname).FirstOrDefault();
                            U.JobID = jobinfo == null ? Guid.Empty : jobinfo.ID;

                            var orgname = Dt.Rows[i][10].ToString();
                            var orginfo = db.Structure.Where(e => e.StructureName == orgname).FirstOrDefault();
                            U.StructureID = orginfo == null ? Guid.Empty : orginfo.ID;

                            list.Add(U);
                            info.Msg = "导入成功";
                        }
                        catch (Exception ex)
                        {
                            info.Msg = "账号为：" + Dt.Rows[i][0].ToString() + "的" + info.RowNumber + "行发生错误：" + ex.Message;
                        }
                        importData.Add(info);
                    }

                    //数据全部添加
                    db.Set<UserInfo>().AddRange(list);
                    if (db.SaveChanges() > 0)
                    {
                        datajson.code = 0;
                        datajson.msg = "导入成功";
                        datajson.data = list;
                    }
                    else
                    {
                        datajson.code = 1;
                        datajson.msg = "导入失败";
                        datajson.data = importData;
                    }

                }
                else
                {
                    datajson.code = 1;
                    datajson.msg = "导入的格式错误";
                    datajson.data = null;
                }
            }
            else
            {
                datajson.code = 1;
                datajson.msg = "未找到需要导入的数据";
                datajson.data = null;
            }

            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        public class ImportMsg
        {
            public int RowNumber { get; set; }

            public string Msg { get; set; }
        }

        //导出
        public ActionResult DeriveBase()
        {
            List<UserInfo> list = db.UserInfo.ToList();

            //将List集合中的内容导出到Excel中
            //1、创建工作簿对象
            IWorkbook wkBook = new HSSFWorkbook();
            //2、在该工作簿中创建工作表对象
            ISheet sheet = wkBook.CreateSheet("人员信息"); //Excel工作表的名称


            IRow BTrow = sheet.CreateRow(0);

            // 设置列名  
            // 假设我们有三个列名："ID", "Name", "Age"  
            ICell cell0 = BTrow.CreateCell(0);
            cell0.SetCellValue("员工姓名");

            ICell cell1 = BTrow.CreateCell(1);
            cell1.SetCellValue("员工编号");

            ICell cell2 = BTrow.CreateCell(2);
            cell2.SetCellValue("性别");

            ICell cell3 = BTrow.CreateCell(3);
            cell3.SetCellValue("身份证号");

            ICell cell4 = BTrow.CreateCell(4);
            cell4.SetCellValue("邮箱");

            ICell cell5 = BTrow.CreateCell(5);
            cell5.SetCellValue("电话");

            ICell cell6 = BTrow.CreateCell(6);
            cell6.SetCellValue("是否离职");

            ICell cell7 = BTrow.CreateCell(7);
            cell7.SetCellValue("籍贯");

            ICell cell8 = BTrow.CreateCell(8);
            cell8.SetCellValue("家庭住址");

            ICell cell9 = BTrow.CreateCell(9);
            cell9.SetCellValue("职业编号");

            ICell cell10 = BTrow.CreateCell(10);
            cell10.SetCellValue("机构编号");

            ICell cell11 = BTrow.CreateCell(11);
            cell11.SetCellValue("创建时间");

            //2.1向工作表中插入行与单元格
            for (int i = 0; i < list.Count; i++)
            {
                //在Sheet中插入创建一行
                IRow row = sheet.CreateRow(i + 1);
                //在该行中创建单元格
                //方式一
                //ICell cell = row.CreateCell(0);
                //cell.SetCellValue(list[i].Name);
                //方式二
                 //给单元格设置值：第一个参数(第几个单元格)；第二个参数(给当前单元格赋值)
                row.CreateCell(0).SetCellValue(list[i].UserName);
                row.CreateCell(1).SetCellValue(list[i].UserCode);
                row.CreateCell(2).SetCellValue(list[i].sex.ToString());
                row.CreateCell(3).SetCellValue(list[i].IDCard);
                row.CreateCell(4).SetCellValue(list[i].Email);
                row.CreateCell(5).SetCellValue(list[i].TelPhone);
                row.CreateCell(6).SetCellValue(list[i].Dimission.ToString());
                row.CreateCell(7).SetCellValue(list[i].Native);
                row.CreateCell(8).SetCellValue(list[i].Address);
                row.CreateCell(9).SetCellValue(list[i].JobID.ToString());
                row.CreateCell(10).SetCellValue(list[i].StructureID.ToString());
                row.CreateCell(11).SetCellValue(list[i].CreateDate.ToString());
            }
            MemoryStream memoryStream = new MemoryStream();
            wkBook.Write(memoryStream);

            // 重置流的位置到开始，以便我们可以读取它  
            memoryStream.Position = 0;

            // 设置HTTP响应的内容类型为Excel  
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=ExportedData.xls");

            return File(memoryStream, "application/vnd.ms-excel", "ExportedData.xls");
        }
    }
}