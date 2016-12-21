using DataAnalysis2.Models;
using DataAnalysis2.UnityTool;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAnalysis2.Controllers
{
    public class HomeController : Controller
    {
 

        /// <summary>
        /// 文件上传页,这个页面还需要有一个文件上传功能 就以同名方法名然后以get和post区分 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpLoad() {
            //当return View后 会自动找到同名cshtml 可以理解吧
            return View();
        }
        /// <summary>
        /// 别问我为啥是这个参数，我也是查的,作用就是接收文件
        /// 参数你还是去搜搜具体怎么写
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpLoad(HttpPostedFileBase file) {
            //文件接收到以后需要做一些处理 
            if (file == null || ".xlsx" != Path.GetExtension(file.FileName))
            {
                return Content("文件格式不正确！", "text/plain");
            }
            var fileName = Path.Combine(Request.MapPath("~/Upload"), "data.xlsx");
           try
           {
                file.SaveAs(fileName);
                ExcelUtility.WriteSteamToFile(ExcelUtility.RenderDataTableToExcel(ExcelUtility.ProcessData(fileName)), fileName);

                return RedirectToAction("detail",new { group = 0 });
            }
            catch(Exception ex)
            {
                //没成功就留在upload页面了
                return View();
            }
            //上传文件后就到概览页了
      
        }
        /// <summary>
        /// 详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail(int group=0) {
            //这里写要显示什么数据
            string path = Path.Combine(Request.MapPath("~/Upload"), "data.xlsx");
            ViewBag.length = ExcelUtility.GetGroupLengthByPath(path);
            ViewBag.current = group;
            DataTable data = ExcelUtility.GetDataByPathAndGroupId(path,group);
            //现在要做的就是把前面的注释补上。然后把这个data以表格形式显示出来
            List<DataClass> list = new List<DataClass>();
            int RowCount = data.Rows.Count;

            DataUtility.CalAB(data);


            for(int i = 0; i < RowCount; i++)
            {
                list.Add(new DataClass { Distance = data.Rows[i][0].ToString(), ZV = data.Rows[i][1].ToString(), FV = data.Rows[i][2].ToString() });
            }

            ViewBag.y1 = DataUtility.a + Convert.ToDouble(data.Rows[0][0]) * DataUtility.b;
            ViewBag.y2 = DataUtility.a+ Convert.ToDouble(data.Rows[RowCount - 1][0]) * DataUtility.b;
            ViewBag.x1 = data.Rows[0][0];
            ViewBag.x2 = data.Rows[RowCount-1][0];
            ViewBag.Expression = "Y = "+ DataUtility.b.ToString("0.0000") + " * X + "+ DataUtility.a.ToString("0.0000");
            ViewBag.xxd = (DataUtility.xxd(data)*100).ToString("0.00")+"%";
            ViewBag.lmd = DataUtility.b.ToString("0.0000");
            ViewBag.cz = (DataUtility.cz(data) * 100).ToString("0.00") + "%";

            double U, Q, S, N, M;
            U = DataUtility.U;
            M = DataUtility.M;
            S = DataUtility.S;
            Q = DataUtility.Q;
            N = DataUtility.N;
            ViewBag.a11 = U.ToString("0.0000");
            ViewBag.a12 = M;
            ViewBag.a13 = (U / M).ToString("0.0000");
            ViewBag.a14 = (U / M / (Q / (N - M - 1))).ToString("0.0000");
            ViewBag.a21 = Q.ToString("0.0000");
            ViewBag.a22 = N - M - 1;
            ViewBag.a23 = (Q / (N - M - 1)).ToString("0.0000");
            ViewBag.a31 = S.ToString("0.0000");
            ViewBag.a32 = N - 1;

            if (group== ExcelUtility.GetGroupLengthByPath(path)-1) ViewBag.cfx = (DataUtility.cfx(ExcelUtility.DataTableFromPath(path)) * 100).ToString("0.00") + "%";
            
            ViewBag.Data = list;
            return View(list);
        }

    }
}