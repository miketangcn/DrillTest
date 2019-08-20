using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillTest.Model
{
   public class Global
    {
        #region 系统设置参数
        public static string IP1;
        public static string IP2;
        public static int con_chek_x;
        public static int con_chek_y;
        public static int min_x;
        public static int min_y;
        public static int max_x;
        public static int max_y;
        public static float con_factor_x;
        public static float con_factor_y;
        public static string SavePath;
        public static string OutputPath;
        #endregion

        public static string DateFilePath;
        public static string DateFileName1;
        public static string DateFileName2;
        public static bool IsShow1, IsShow2;
        public static Int16 MaxPressure1=0, MaxPressure2=0;//最大压力
        public static CurrentPoint CurrentPoint1=new CurrentPoint();//转换后的显示值
        public static CurrentPoint CurrentPoint2 = new CurrentPoint();//转换后的显示值
        public static CurrentHoleCount CurrentHoleCount1 = new CurrentHoleCount();//显示用的当前洞的编号
        public static CurrentHoleCount CurrentHoleCount2 = new CurrentHoleCount();//显示用的当前洞的编号
        public  static Point Point1 = new Point();//#1当前数据
        public  static Point Point2 = new Point();//#2当前数据
        public static bool ConnectStatus1;
        public static bool ConnectStatus2;
        public static bool Working1;
        public static bool Working2;
        public static bool SubWorking1;
        public static bool SubWorking2;
        public static bool LastSubWorking1;
        public static bool LastSubWorking2;
        public static List<Point> lstPoint1 = new List<Point>();//一个洞的测试数据
        public static List<Point> lstPoint2 =  new List<Point>();
        public static Int16 HoleNumber1;
        public static Int16 HoleNumber2;
        public static WorkRecord WorkRecord1 = new WorkRecord();
        public static WorkRecord WorkRecord2 = new WorkRecord();
        public static HoleRecod HoleRecod1 = new HoleRecod();
        public static HoleRecod HoleRecod2 = new HoleRecod();
        public static Int16 i, j;
        public static bool flag = false;
        public static bool LastQueryMode;
        public static DateTime LastQueryTFrom, LastQueryTTo;
        public static string LastQueryId;
        public static int LastDataRow;
    }
}

