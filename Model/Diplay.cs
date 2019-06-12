using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrillTest.MVVMBase;

namespace DrillTest.Model
{
  public class CurrentPoint:ViewModelsBase
  {

        private string _x="0.00";

        public string X
        {
            get { return _x; }
            set { _x = value; RasiePropertyChanged(); }
        }
        private string _y="0.00";

        public string Y
        {
            get { return _y; }
            set { _y = value; RasiePropertyChanged(); }
        }
  }
    public class CurrentHoleCount:ViewModelsBase
    {
        private string  _count;

        public string  Count
        {
            get { return _count; }
            set { _count = value; RasiePropertyChanged(); }
        }

    }
}
