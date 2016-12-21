using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.HSSF.Util;
using NPOI.HSSF.Extractor;
using System.Web.UI.HtmlControls;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;

namespace DataAnalysis2.UnityTool
{
    public class ExcelUtility
    {

        /// <summary>
        /// 從位元流讀取資料到DataTable.
        /// </summary>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int StartCell)
        {
            IWorkbook workbook = null;
            //workbook = new HSSFWorkbook(ExcelFileStream);
            try
            {
                workbook = new XSSFWorkbook(ExcelFileStream);
            }
            catch (Exception ex)
            {
                workbook = new HSSFWorkbook(ExcelFileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("0", typeof(decimal)));
            table.Columns.Add(new DataColumn("1", typeof(decimal)));
            table.Columns.Add(new DataColumn("2", typeof(decimal)));
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = 0; j <= 2; j++)
                {
                    if (row.GetCell(StartCell+j) != null)
                    {
                        ICell cellN = row.GetCell(StartCell + j);
                        //dataRow[j] = row.GetCell(StartCell + j).ToString();
                        dataRow[j] = cellN.ToString();
                    }
                        
                }

                table.Rows.Add(dataRow);
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        public static DataTable ProcessData(string path)
        {
            Stream ExcelFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;
            try
            {
                workbook = new XSSFWorkbook(ExcelFileStream);
            }
            catch (Exception ex)
            {
                workbook = new HSSFWorkbook(ExcelFileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);

            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for(int i = 0; i < cellCount+3; i++)
            {
                table.Columns.Add(new DataColumn(i.ToString(), typeof(decimal)));
            }
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();
                double x, y1, y2;
                int j;
                x = 0;y1 = 0;y2 = 0;
                for (j = 0; j < row.LastCellNum; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        dataRow[j] = row.GetCell(j).NumericCellValue;
                    }
                    if (j % 3 == 0) x = x+row.GetCell(j).NumericCellValue;
                    if (j % 3 == 1) y1 = y1 + row.GetCell(j).NumericCellValue;
                    if (j % 3 == 2) y2 = y2 + row.GetCell(j).NumericCellValue;
                }
                dataRow[j++] = x / (cellCount / 3);
                dataRow[j++] = y1 / (cellCount / 3);
                dataRow[j] = y2 / (cellCount / 3);
                table.Rows.Add(dataRow);
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;

        }

        public static DataTable DataTableFromPath(string path)
        {
            Stream ExcelFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;
            try
            {
                workbook = new XSSFWorkbook(ExcelFileStream);
            }
            catch (Exception ex)
            {
                workbook = new HSSFWorkbook(ExcelFileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);

            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int i = 0; i < cellCount; i++)
            {
                table.Columns.Add(new DataColumn(i.ToString(), typeof(decimal)));
            }
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();
                for (int j = 0; j < row.LastCellNum; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        dataRow[j] = row.GetCell(j).ToString();
                    }
                }
                table.Rows.Add(dataRow);
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;

        }

        public static Stream RenderDataTableToExcel(DataTable SourceTable)
        {
            // SourceTable = ProcessData(path);

            IWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
           // foreach (DataColumn column in SourceTable.Columns)
               // headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

            // handling value.
            int rowIndex = 0;

            foreach (DataRow row in SourceTable.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in SourceTable.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return ms;
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
        public static void WriteSteamToFile(Stream s, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            MemoryStream ms = new MemoryStream();
            CopyStream(s, ms);
            byte[] data = ms.ToArray();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();

            data = null;
            ms = null;
            fs = null;
        }

        /// <summary>
        /// path为文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetGroupLengthByPath(string path)
        {
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;
            //workbook = new HSSFWorkbook(ExcelFileStream);
            try
            {
                workbook = new XSSFWorkbook(stream);
            }
            catch (Exception ex)
            {
                workbook = new HSSFWorkbook(stream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);//第一行为标题行
            int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells

            return cellCount/3;
        }
        public static DataTable GetDataByPathAndGroupId(string path, int GroupId)
        {
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            int StartCell = 0;
            StartCell = GroupId * 3;
            return RenderDataTableFromExcel(stream,StartCell);
        }
    }
    
}