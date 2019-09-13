using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using NPOI.POIFS.FileSystem;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.OpenXml4Net;
using System.Reflection;
using DrillTest.Model;


namespace DrillTest.Lib
{
    public class NopiExcelHelper<T>
    {
        public static void AddExcel(List<T> lists, string workbookFile,string SheetNumber)
        {
            try
            {
                int sheetflat = -1;
                FileStream fs = new FileStream(workbookFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//读取流
                POIFSFileSystem ps = new POIFSFileSystem(fs);
                fs.Close();
                FileStream fout = new FileStream(workbookFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);//写入流
                HSSFWorkbook workbook = new HSSFWorkbook(ps);
                HSSFSheet sheet=null ; 
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    if (SheetNumber==workbook.GetSheetName(i))
                    {
                        sheetflat = i;
                    }
                }
                if (sheetflat!=-1)
                {
                    sheet = workbook.GetSheetAt(sheetflat) as HSSFSheet;//如果已经有了就先清空该sheet
                    for (int i = 0; i < sheet.LastRowNum+1; i++)
                    {
                        HSSFRow row = sheet.GetRow(i) as HSSFRow;
                        row.CreateCell(0).SetCellValue("");
                        row.CreateCell(1).SetCellValue("");
                    }
                }
                else
                {
                    sheet = workbook.CreateSheet(SheetNumber) as HSSFSheet;
                }

                HSSFRow HeadRow = sheet.CreateRow(0) as HSSFRow;
                bool h = false;
                int j = 1;
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();//获得类中的各个属性，添到表单中
                foreach (T item in lists)//泛型的类
                {
                    HSSFRow dataRow = sheet.CreateRow(j) as HSSFRow;
                    int i = 0;
                    foreach (PropertyInfo column in properties)
                    {
                        if (!h)
                        {
                            HeadRow.CreateCell(i).SetCellValue(column.Name);//在第一行数据前加入列标签
                            if (i == 0)
                            {
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" :
                               (Convert.ToInt16(column.GetValue(item, null)) * Global.con_factor_x).ToString("000.00"));
                            }
                            else
                            {
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" :
                               (Convert.ToInt16(column.GetValue(item, null)) * Global.con_factor_y).ToString("000.00"));
                            }

                        }
                        else
                        {
                            if (i == 0)
                            {
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" :
                             //  (Convert.ToInt16(column.GetValue(item, null)) * Global.con_factor_x).ToString("000.00"));
                                (Convert.ToInt16(column.GetValue(item, null)) * 1).ToString());
                            }
                            else
                            {
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" :
                              // (Convert.ToInt16(column.GetValue(item, null)) * Global.con_factor_y).ToString("000.00"));
                                (Convert.ToInt16(column.GetValue(item, null)) * 1).ToString());
                            }
                        }

                        i++;
                    }
                    h = true;
                    j++;
                }

                fout.Flush();
                workbook.Write(fout);//写入文件
                workbook = null;
                fout.Close();

            }
            catch (Exception ee)
            {
                string see = ee.Message;
            }
        }

    }
}
