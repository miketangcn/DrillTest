using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.ModBus;
using DrillTest.Model;
using System.Threading;

namespace DrillTest.Lib
{
   public static class ReadValue
    {
        private static ModbusTcpNet ModbusTcpNet1 = new ModbusTcpNet(Global.IP2);
        private static ModbusTcpNet ModbusTcpNet2 = new ModbusTcpNet(Global.IP1);
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
            //Global.i = 900;
            //Global.j = 150;
            string address = "x=4;72";
            while (true)
            {
                if (!Global.ConnectStatus1)
                {
                    ModbusTcpNet1.ConnectServer();
                }
                try
                {
                    OperateResult<byte[]> result = ModbusTcpNet1.Read(address, ushort.Parse("2"));
                    if (result.IsSuccess)
                    {
                        Global.ConnectStatus1 = true;
                        Point point = new Point();
                        //Global.i = (short)(Global.i + 2);
                        //if (Global.j > 400)
                        //{
                        //    Global.flag = true;
                        //}
                        //if (Global.flag)
                        //{
                        //    Global.j = (short)(Global.j - 8);
                        //}
                        //else
                        //{
                        //    Global.j++;
                        //}
                        //Global.Point1.x = Global.i;
                        //Global.Point1.y = Global.j;
                        //point.x = Global.i;
                        //point.y = Global.j;
                        //point.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                        //point.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 1);
                        Global.Point1.x = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 0);
                        Global.Point1.y = ModbusTcpNet1.ByteTransform.TransInt16(result.Content, 2);
                        Global.ConnectStatus1 = true;
                        CommonMethods.DateTreating1(point);
                    }
                    else Global.ConnectStatus1 = false;
                }
                catch
                {
                    //设置读写标志为false   
                    Global.ConnectStatus1 = false;
                }
               Thread.Sleep(10);   
            }
        }
        private static void ThreadBackgroundRead2()
        {
            string address = "x=4;72";
            while (true)
            {
                if (!Global.ConnectStatus2)
                {
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
                        CommonMethods.DateTreating2(Global.Point2);
                    }
                    else Global.ConnectStatus2 = false;
                }
                catch
                {
                    //设置读写标志为false  
                    Global.ConnectStatus2 = false;
                }
                Thread.Sleep(10);
            }
        }
    }
}
