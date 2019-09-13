using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DrillTest.Model;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;


namespace DrillTest.Lib
{
    public static class CommonMethods
    {
        //模板文件全路径
        private static string ModelFileName = System.Windows.Forms.Application.StartupPath + @"\Template\Template.xls";
        
        #region 读写配置文件
        public static void ReadConfig()
        {
            Model.Global.IP1 = System.Configuration.ConfigurationManager.AppSettings["IP1"];
            Model.Global.IP2 = System.Configuration.ConfigurationManager.AppSettings["IP2"];
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["con_chek_x"], out Model.Global.con_chek_x);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["con_chek_y"], out Model.Global.con_chek_y);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["min_x"], out Model.Global.min_x);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["max_x"], out Model.Global.max_x);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["min_y"], out Model.Global.min_y);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["max_y"], out Model.Global.max_y);
            float.TryParse(System.Configuration.ConfigurationManager.AppSettings["con_factor_x"], out Model.Global.con_factor_x);
            float.TryParse(System.Configuration.ConfigurationManager.AppSettings["con_factor_y"], out Model.Global.con_factor_y);
            Global.SavePath = System.Configuration.ConfigurationManager.AppSettings["SavePath"];
            Global.OutputPath = System.Configuration.ConfigurationManager.AppSettings["OutputDirect"];
        }
        public static void WriteConfig(string OutputDirect)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["OutputDirect"].Value = OutputDirect;
                config.Save();
                ConfigurationManager.RefreshSection("OutputDirect");
                Global.OutputPath = OutputDirect;
            }
            catch (Exception)
            {
                throw;
            }
          
        }
        #endregion

        #region 新建文件夹及新建文件
        public static bool GetFilePath(string FileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                    if (CopyExcelModel(FileName))
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (CopyExcelModel(FileName))
                    return true;
                else return false;

            }
        }
        /// <summary>
        /// 拷贝模板到目标文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool CopyExcelModel(string FileName)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(ModelFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ModelFileName));
                }
                if (!File.Exists(FileName))
                {
                    if (File.Exists(ModelFileName))
                    {
                        File.Copy(ModelFileName, FileName);
                        return true;
                    }
                    else return false;
                }
                else return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 采集数据后处理
        public static void DateTreating1(Point point)
        {
            string FullFileName = Global.DateFilePath + Global.DateFileName1;
            //测试状态
            if (Global.Working1)
            {
                if (Global.Point1.x >= Global.con_chek_x && Global.Point1.y >= Global.con_chek_y)//进入压洞状态
                {
                    Global.SubWorking1 = true;
                    FrmTest.frmtest.SetRedo1Ena(false);
                    if (Global.Point1.y>Global.MaxPressure1)
                    {
                        Global.MaxPressure1 = Global.Point1.y;
                    }      
                    Global.lstPoint1.Add(point);
                }
                else if (Global.Point1.x <Global.con_chek_x || Global.Point1.y < Global.con_chek_y)//不在压洞状态
                {
                    Global.SubWorking1 = false;
                }

                if (Global.LastSubWorking1 && !Global.SubWorking1)//一个洞压完
                {
                    //调用异步写数据库和写数据文件代码
                    Global.WorkRecord1.LastTime = DateTime.Now;
                    Global.WorkRecord1.HoleCount = Global.HoleNumber1;
                    Global.HoleRecod1.MaxPressure = (float)Math.Round((Global.MaxPressure1 * Global.con_factor_y - 10), 2);
                    Global.HoleRecod1.TestTime = DateTime.Now;
                    Global.HoleRecod1.Id = Global.WorkRecord1.Id;
                    Global.HoleRecod1.HoleNumber = Global.WorkRecord1.HoleCount;
                    Global.HoleRecod1.HoleDate = SerializeListCompress(Global.lstPoint1.ConvertAll(s => (object)s));
                    Global.HoleRecod1.MacId = 1;
                   // Global.HoleRecod1.HoleDate = ToBinary(Global.lstPoint1.ConvertAll(s => (object)s));
                    HoleRecordUpdate(Global.HoleRecod1);
                    WorkTableUpdate(Global.WorkRecord1);
                    if (GetFilePath(FullFileName))
                    {
                        NopiExcelHelper<Point>.AddExcel(Global.lstPoint1, FullFileName, "hole" + Global.HoleNumber1.ToString());
                    }
                    FrmTest.frmtest.SetRedo1Ena(true);
                    Global.MaxPressure1 = 0;
                    Global.HoleNumber1++;
                    #region 要删除
                    ReadValue.Distance = 50;
                    ReadValue.Pressure = 350;
                    ReadValue.IsMax = false;
                    #endregion
                }
                if (!Global.SubWorking1)
                {
                    Global.lstPoint1.Clear();
                }

              Global.LastSubWorking1 = Global.SubWorking1;
 
            }
            //非测试状态
            else
            {
                Global.lstPoint1.Clear();
            }
        }

        public static void DateTreating2(Point point)
        {
            string FullFileName = Global.DateFilePath + Global.DateFileName2;
            if (Global.Working2)
            {
                if (Global.Point2.x >= Global.con_chek_x && Global.Point2.y >= Global.con_chek_y)//进入压洞状态
                {
                    Global.SubWorking2 = true;
                    FrmTest.frmtest.SetRedo2Ena(false);
                    Global.lstPoint2.Add(point);
                    if (Global.Point2.y > Global.MaxPressure2)
                    {
                        Global.MaxPressure2 = Global.Point2.y;
                    }
                }
                if (Global.Point2.x < Global.con_chek_x || Global.Point2.y < Global.con_chek_y)//不在压洞状态
                {
                    Global.SubWorking2 = false;
                }
                if (Global.LastSubWorking2 && !Global.SubWorking2)//一个洞压完
                {
                    //调用异步写数据库和写数据文件代码
                    Global.WorkRecord2.LastTime = DateTime.Now;
                    Global.WorkRecord2.HoleCount = Global.HoleNumber2;
                    Global.HoleRecod2.TestTime = DateTime.Now;
                    Global.HoleRecod2.MaxPressure =(float)Math.Round((Global.MaxPressure2 * Global.con_factor_y - 10), 2);
                    Global.HoleRecod2.Id = Global.WorkRecord2.Id;
                    Global.HoleRecod2.HoleNumber = Global.WorkRecord2.HoleCount;
                    Global.HoleRecod2.HoleDate = SerializeListCompress(Global.lstPoint2.ConvertAll(s => (object)s));
                    Global.HoleRecod2.MacId = 1;
                    WorkTableUpdate(Global.WorkRecord2);
                    HoleRecordUpdate(Global.HoleRecod2);
                    if (GetFilePath(FullFileName))
                    {
                        NopiExcelHelper<Point>.AddExcel(Global.lstPoint2, FullFileName, "hole" + Global.HoleNumber2.ToString());
                    }
                    FrmTest.frmtest.SetRedo2Ena(true);
                    Global.MaxPressure2 = 0;
                    Global.HoleNumber2++;
                }
                if (!Global.SubWorking2)
                {
                    Global.lstPoint2.Clear();
                }
                Global.LastSubWorking2 = Global.SubWorking2;
            }
            //非测试状态
            else
            {
                Global.lstPoint2.Clear();
            }
        }
        #endregion

        #region 数据库及数据文件处理

        private static void WorkTableUpdate(WorkRecord workRecord)
        {
            //string sql = @"MERGE Work AS target USING (SELECT @SerialNO as SerialNO, @MachineId as MachineId, @Layer as Layer, @HoleCount as HoleCount, 
            //            @LastTime as LastTime)  AS source ON (target.SerialNO = source.SerialNO) WHEN MATCHED THEN UPDATE SET 
            //            HoleCount = source.HoleCount  WHEN NOT MATCHED THEN INSERT (SerialNO, MachineId, Layer, HoleCount, LastTime) 
            //            VALUES (source.SerialNO, source.MachineId, source.Layer, source.HoleCount, source.LastTime);";
            //SqlParameter[] param = { new SqlParameter("@SerialNO",workRecord.Id), new SqlParameter("@MachineId", workRecord.MachineId ), new SqlParameter("@Layer", workRecord.Layer ),
            //            new SqlParameter("@HoleCount",workRecord.HoleCount ), new SqlParameter( "@LastTime",workRecord.LastTime ) };
            string sql = @"MERGE Work AS target USING (SELECT @SerialNO as SerialNO, @Layer as Layer, @HoleCount as HoleCount, 
                        @LastTime as LastTime)  AS source ON (target.SerialNO = source.SerialNO) WHEN MATCHED THEN UPDATE SET 
                        HoleCount = source.HoleCount  WHEN NOT MATCHED THEN INSERT (SerialNO, Layer, HoleCount, LastTime) 
                        VALUES (source.SerialNO,  source.Layer, source.HoleCount, source.LastTime);";
            SqlParameter[] param = { new SqlParameter("@SerialNO",workRecord.Id),  new SqlParameter("@Layer", workRecord.Layer ),
                        new SqlParameter("@HoleCount",workRecord.HoleCount ), new SqlParameter( "@LastTime",workRecord.LastTime ) };
            SQLHelper.Update(sql, param);
        }
        private static void HoleRecordUpdate(HoleRecod holeRecod)
        {
            string sql = @"MERGE HoleTestRec AS target USING (SELECT @SerialNO as SerialNO , @HoleNumber as HoleNumber, 
                        @MaxPressure as MaxPressure, @TestTime as TestTime , @Data as Data , @MacId as MacId ,@LayerNo as LayerNo)  AS source ON               
                        (target.SerialNO = source.SerialNO  and target.HoleNumber = source.HoleNumber )
                        WHEN MATCHED THEN UPDATE SET TestTime=source.TestTime, Data = source.Data, MaxPressure=source.MaxPressure
                        WHEN NOT MATCHED THEN INSERT (SerialNO, HoleNumber, MaxPressure,  TestTime, Data ,MacId , LayerNo) 
                        VALUES (source.SerialNO, source.HoleNumber, source.MaxPressure, source.TestTime,source.Data , source.MacId , source.LayerNo);";
            SqlParameter[] param = { new SqlParameter("@SerialNO",holeRecod.Id), new SqlParameter("@HoleNumber",holeRecod.HoleNumber ),
                       new SqlParameter("@MaxPressure",holeRecod.MaxPressure ), new SqlParameter("@TestTime",holeRecod.TestTime ),
                       new SqlParameter( "@Data",holeRecod.HoleDate),new SqlParameter("@MacId",holeRecod.MacId),new SqlParameter("@LayerNo",holeRecod.LayerNo)};
            SQLHelper.Update(sql, param);
        }
        #endregion

        #region list对象序列化到字符串和字符串反序列化 两种方法
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }

        public static string SerializeListCompress(List<object> list)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream, list);
                stream.Position = 0;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
                byte[] compressAfterByte = Compress(bytes);
                return Convert.ToBase64String(compressAfterByte);
                //byte[] bytes=stream.GetBuffer();
                //return Encoding.ASCII.GetString(bytes,0,bytes.Length);
            }
        }
        public static List<object> DesirializeListCompress(String data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes = Convert.FromBase64String(data);
            bytes= Decompress(bytes);
            try
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    List<object> list = bf.Deserialize(stream) as List<object>;
                    stream.Flush();
                    stream.Close();
                    return list;
                }

            }
            catch
            {
                return null;
            }
            //byte[] bytes = Encoding.ASCII.GetBytes(data);
        }

        public static string SerializeList(List<object> list)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream,list);
                stream.Position = 0;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
                return Convert.ToBase64String(bytes);
                //byte[] bytes=stream.GetBuffer();
                //return Encoding.ASCII.GetString(bytes,0,bytes.Length);
            }
        }
        public static string ToBinary(List<object> list)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream, list);
                stream.Position = 0;
                byte[] bytes = stream.ToArray();
                StringBuilder sb = new StringBuilder();
                foreach (byte bt in bytes)
                {
                    sb.Append(string.Format("{0:X2}", bt));//转化为两个16进制数字的字符
                }
                return sb.ToString();
            }
        }
        public static List<object> DesirializeList(String data)
        {
            if (string.IsNullOrEmpty(data))
            return null;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes = Convert.FromBase64String(data);
            try
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    List<object> list = bf.Deserialize(stream) as List<object>;
                    stream.Flush();
                    stream.Close();
                    //string compress= SerializeListCompress(list);
                    //List<object> ListCompress = DesirializeListCompress(compress);
                    //return ListCompress;
                    return list;
                }
            }
            catch 
            {
                return null;
            }
            //byte[] bytes = Encoding.ASCII.GetBytes(data);
        }
        /// <summary>
        /// BinaryFormatter反序列化
        /// </summary>
        /// <param name="str">字符串序列</param>
        public static List<object> FromBinary(string str)
        {
            int intLen = str.Length / 2;//序列化时一个byte变成了两个16进制的字符所以除以2
            byte[] bytes = new byte[intLen];
            for (int i = 0; i < intLen; i++)
            {
                int ibyte = Convert.ToInt32(str.Substring(i * 2, 2), 16);//将16进制的2个字符转化为int类型
                bytes[i] = (byte)ibyte;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return bf.Deserialize(stream) as List<object>;
            }
        }
        #endregion
    }
}
