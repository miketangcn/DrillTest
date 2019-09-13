using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.ModBus;
using HslCommunication.LogNet;
using DrillTest.Model;
using System.Threading;

namespace DrillTest.Lib
{
   public static class ReadValue
    {
        private static ModbusTcpNet ModbusTcpNet1 = new ModbusTcpNet(Global.IP2);
        private static ModbusTcpNet ModbusTcpNet2 = new ModbusTcpNet(Global.IP1);
        public static short Distance, Pressure;
        public static bool IsMax;
        public static ILogNet logNet = new LogNetFileSize(System .Windows.Forms.Application.StartupPath + "\\Logs", 2 * 1024 * 1024);
        private static System.Threading.Thread thread1 = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadBackgroundRead1));
        private static System.Threading.Thread thread2 = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadBackgroundRead2));
        public static async void  ConnnectPlc1()
        {
            Task task = new Task(() =>
            {
                try
                {
                    OperateResult connect1 = ModbusTcpNet1.ConnectServer();
                    if (connect1.IsSuccess)
                    {
                        Global.ConnectStatus1 = true;
                    }
                    else Global.ConnectStatus1 = false;
                }
                catch
                {
                    Global.ConnectStatus1 = false;
                }

            });
            task.Start();
            await task;
        }
        public  static async void ConnnectPlc2()
        {
            Task task=new Task(()=>
            {
                try
                {
                    OperateResult connect2 = ModbusTcpNet2.ConnectServer();
                    if (connect2.IsSuccess)
                    {
                        Global.ConnectStatus2=true;
                    }
                    else Global.ConnectStatus2 = false;
                }
                catch
                {
                    Global.ConnectStatus2 = false;
                }

            });
            task.Start();
            await task;
           
           
        }
        public static void DisConnnectPlc1()
        {
            ModbusTcpNet1.ConnectClose();
        }
        public static void DisConnnectPlc2()
        {
            ModbusTcpNet2.ConnectClose();
        }
        public static void StartRead1()
        {
           
            thread1.IsBackground = true;
            thread1.Start();

        }
        public static void StartRead2()
        {
            thread2.IsBackground = true;
            thread2.Start();

        }
        private static void ThreadBackgroundRead1()
        {
            string address = "x=4;72";
            string addresswrite = "96";//要删掉
            
            while (true)
            {
                if (!Global.ConnectStatus1)
                {
                    logNet.WriteWarn("#1压机通讯故障");
                    ModbusTcpNet1.ConnectServer();
                }
                try
                {
                    #region 写AO1,AO2 这一段要删掉
                    if (Global.Working1)
                    {
                        Distance = (short)(Distance + 2);
                        if (Pressure < 650 && !IsMax)
                        {
                            Pressure = (short)(Pressure + 1);
                        }
                        else
                        {
                            IsMax = true;
                            Pressure = (short)(Pressure - 4);
                        }
                        byte[] buffer = new byte[4];
                        ModbusTcpNet1.ByteTransform.TransByte(Distance).CopyTo(buffer, 0);//
                        ModbusTcpNet1.ByteTransform.TransByte(Pressure).CopyTo(buffer, 2);
                        OperateResult write = ModbusTcpNet1.Write(addresswrite, buffer);//要删掉
                        if (write.IsSuccess)
                        {
                            OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                            if (result.IsSuccess)
                            {
                                Global.ConnectStatus1 = true;
                                Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                                Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                                Point point = new Point();
                                point.x = Global.Point1.x;
                                point.y = (short)(Global.Point1.y - 1000);
                                CommonMethods.DateTreating1(point);
                            }
                            else Global.ConnectStatus1 = false;
                        }
                        else
                        {
                            // failed
                        }
                    }
                    #endregion
                    //OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                    //if (result.IsSuccess)
                    //{
                    //    Global.ConnectStatus1 = true;
                    //    Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                    //    Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                    //    Point point = new Point();
                    //    point.x = Global.Point1.x;
                    //    point.y = (short)(Global.Point1.y - 1000);
                    //    CommonMethods.DateTreating1(point);
                    //}
                    //else Global.ConnectStatus1 = false; //这段后面要恢复
                }
                catch
                {
                    //设置读写标志为false   
                    Global.ConnectStatus1 = false;
                }
               Thread.Sleep(2);   
            }
        }
        private static void ThreadBackgroundRead2()
        {
            string address = "x=4;72";
            while (true)
            {
                if (!Global.ConnectStatus2)
                {
                    logNet.WriteWarn("#2压机通讯故障");
                    ModbusTcpNet2.ConnectServer();
                }
                try
                {
                    OperateResult<Byte[]> result = ModbusTcpNet2.Read(address, ushort.Parse("2"));
                    if (result.IsSuccess)
                    {
                        Global.Point2.x = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 0);
                        Global.Point2.y = ModbusTcpNet2.ByteTransform.TransInt16(result.Content, 2);
                        Global.ConnectStatus2 = true;
                        Point point = new Point();
                        point.x = Global.Point2.x;
                        point.y = (short)(Global.Point2.y - 1000);
                        CommonMethods.DateTreating2(point);
                    }
                    else Global.ConnectStatus2 = false;
                }
                catch
                {
                    //设置读写标志为false  
                    Global.ConnectStatus2 = false;
                }
                Thread.Sleep(5);
            }
        }
    }
}
