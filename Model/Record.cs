using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillTest.Model
{
   public class WorkRecord
    {
        public string Id { get; set; } //零件编号
        public Int16 Layer { get; set; }//层数
        public Int16 MachineId { get; set; } //测试机器编号
        public DateTime LastTime { get; set; }//记录时间
        public Int16 HoleCount { get; set; }//洞数
    }

    public class HoleRecod
    {
        public string Id { get; set; } //零件编号
        public Int16 HoleNumber { get; set; } //洞编号
        public float MaxPressure { get; set; } //最大压力
        public DateTime TestTime { get; set; }//记录时间
        public string HoleDate { get; set; }
    }
    [Serializable]
    public class Point
    {
        public Int16 x { get; set; } //位移读数
        public Int16 y { get; set; } //压力读数
    }


}
