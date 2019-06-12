using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DrillTest.MVVMBase
{
    public class ViewModelsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RasiePropertyChanged([CallerMemberName]string VarName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(VarName));
        }

    }

}
