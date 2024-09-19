using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SXHP_HengJiuGame_WenJH.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NpoiHelper
    {
        public static DataTable Import(string strFileName, string FileType)
        {
            IWorkbook hssfworkbook;
            DataTable dataTable = new DataTable();

            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (FileType == "xls")
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                else
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }

            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dataTable.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dataTable.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    var ddd = row.GetCell(j);
                    if (row.GetCell(j) != null)
                    {
                        dataRow[j] = row.GetCell(j).ToString();
                    }
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
    }
}