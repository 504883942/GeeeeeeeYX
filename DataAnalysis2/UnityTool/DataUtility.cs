using DataAnalysis2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DataAnalysis2.UnityTool
{
    public class DataUtility
    {
        public static double a, b, sxx, sxy, syy, S, U, Q, N, M;
        public static void CalAB(DataTable data)
        {
            int RowCount = data.Rows.Count;
            double lx, ly, lxx,lyy,lxy;
            lx = 0;ly = 0;lxx = 0;lyy = 0;lxy = 0;
            for (int i = 0; i < RowCount; i++)
            {
                double x, y;
                x = Convert.ToDouble(data.Rows[i][0]);
                y = (Convert.ToDouble(data.Rows[i][1])+ Convert.ToDouble(data.Rows[i][2]))/2;
                lx = lx + x;
                ly = ly + y;
                lxx = lxx + x * x;
                lyy = lyy + y * y;
                lxy = lxy + x * y;
            }
            sxx = lxx - lx * lx / RowCount;
            syy = lyy - ly * ly / RowCount;
            sxy = lxy - lx * ly / RowCount;
            b = sxy / sxx;
            a = (ly / RowCount) - b * (lx / RowCount);

            S = syy;
            U = b * sxy;
            Q = S - U;
            N = RowCount;
            M = 1;
        }

        public static double xxd(DataTable data)
        {
            double yfs, ym;
            int RowCount = data.Rows.Count;
            yfs = b * (Convert.ToDouble(data.Rows[RowCount-1][0])- Convert.ToDouble(data.Rows[0][0]));
            ym = 0;
            for (int i = 0; i < RowCount; i++)
            {
                double x, y0,y1,y2,y;
                x = Convert.ToDouble(data.Rows[i][0]);
                y1 = Convert.ToDouble(data.Rows[i][1]);
                y2 = Convert.ToDouble(data.Rows[i][2]);
                y0 = (y1 + y2) / 2;
                y = a + b * x;
                if (Math.Abs(y0 - y) > ym) ym = Math.Abs(y0 - y);
            }
            return ym/yfs;

        }

        public static double cz(DataTable data)
        {
            double yfs, ym;
            int RowCount = data.Rows.Count;
            yfs = b * (Convert.ToDouble(data.Rows[RowCount - 1][0]) - Convert.ToDouble(data.Rows[0][0]));
            ym = 0;
            for (int i = 0; i < RowCount; i++)
            {
                double y1, y2;
                y1 = Convert.ToDouble(data.Rows[i][1]);
                y2 = Convert.ToDouble(data.Rows[i][2]);
                if (Math.Abs(y1 - y2) > ym) ym = Math.Abs(y1 - y2);
            }
            return ym / yfs;
        }

        public static double cfx(DataTable data)
        {
            double yfs, ym;
            int RowCount = data.Rows.Count;
            yfs = b * (Convert.ToDouble(data.Rows[RowCount - 1][0]) - Convert.ToDouble(data.Rows[0][0]));
            ym = 0;
            for (int i = 0; i < RowCount; i++)
            {
                double yp1, yp2,c1,c2;
                yp1 = Convert.ToDouble(data.Rows[i][data.Columns.Count - 2]);
                yp2 = Convert.ToDouble(data.Rows[i][data.Columns.Count - 1]);
                c1 = 0;c2 = 0;
                for (int j = 0; j < data.Columns.Count-3;j=j+3)
                {
                    double y1, y2;
                    y1 = Convert.ToDouble(data.Rows[i][j+1]);
                    y2 = Convert.ToDouble(data.Rows[i][j+2]);
                    c1 = c1 + (y1 - yp1) * (y1 - yp1);
                    c2 = c2 + (y2 - yp2) * (y2 - yp2);
                }
                c1 = Math.Sqrt(c1/(data.Columns.Count / 3-1));
                c2 = Math.Sqrt(c2 / (data.Columns.Count / 3 - 1));
                if (c1 > ym) ym = c1;
                if (c2 > ym) ym = c2;
            }
            return 2 * ym / yfs;
        }
        
    }
}