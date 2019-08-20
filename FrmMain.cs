using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrillTest.Model;
using DrillTest.Lib;

namespace DrillTest
{
    public partial class FrmMain : Form
    {
        private Timer timer = new System.Windows.Forms.Timer();
        private List<Model.Point> points = new List<Model.Point>();

        public FrmMain()
        {
            InitializeComponent();
            this.Load += FrmMain_Load;
           
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            #region 判断系统是否已启动

            System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);//获取指定的进程名   
            if (myProcesses.Length > 1) //如果可以获取到知道的进程名则说明已经启动
            {
                MessageBox.Show("程序已启动！");
                Application.Exit();              //关闭系统
            }

            #endregion
            CommonMethods.ReadConfig();
            if (Global.SavePath=="")
            {
                Global.SavePath = Application.StartupPath + @"\Data\"; 
            }
            Global.DateFilePath = Global.SavePath + DateTime.Now.Date.ToString("yyyy")+@"\"+DateTime.Now.Date.ToString("yyyyMM") + @"\";
            ReadValue.ConnnectPlc1();
            ReadValue.ConnnectPlc2();
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.StartPosition = FormStartPosition.CenterScreen;
            检测界面ToolStripMenuItem_Click(null, null);
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            timer.Start();
            ReadValue.StartRead1();
            ReadValue.StartRead2();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Global.CurrentPoint1.X = (Global.con_factor_x * Global.Point1.x).ToString("000.00");
            Global.CurrentPoint1.Y = (Global.con_factor_y * Global.Point1.y-10).ToString("00.00");
            Global.CurrentPoint2.X = (Global.con_factor_x * Global.Point2.x).ToString("000.00");
            Global.CurrentPoint2.Y = (Global.con_factor_y * Global.Point2.y-10).ToString("00.00");
            Global.CurrentHoleCount1.Count = Global.HoleNumber1.ToString();
            Global.CurrentHoleCount2.Count = Global.HoleNumber2.ToString();
           // points.Add(Global.Point1);
            if (Global.ConnectStatus1)
            {
                lblConnect1Status.Text = "#1机在线！";
                lblConnect1Status.ForeColor = Color.Black;
            }
            else
            {
                lblConnect1Status.Text = "#1机脱机！";
                lblConnect1Status.ForeColor = Color.Red;
            }
            if (Global.ConnectStatus2)
            {
                lblConnect2Status.Text = "#2机在线！";
                lblConnect2Status.ForeColor = Color.Black;
            }
            else
            {
                lblConnect2Status.Text = "#2机脱机！";
                lblConnect2Status.ForeColor = Color.Red;
            }
        }
        #region 窗体操作
        /// <summary>
        /// 判断MainPanel是否有指定窗体名称的窗体，如果有返回True,否则关闭窗体，返回False
        /// </summary>
        /// <param name="FrmName"></param>
        /// <returns></returns>
        private bool CloseWindow(string FrmName)
        {
            bool Res = false;
            foreach (Control ct in this.panel1.Controls)
            {
                if (ct is Form)
                {
                    Form Frm = (Form)ct;
                    if (Frm.Name == FrmName)
                    {
                        Res = true;
                        break;
                    }
                    else
                    {
                        Frm.Close();
                    }

                }
            }
            return Res;
        }

        /// <summary>
        /// 格式化打开窗体
        /// </summary>
        /// <param name="Frm"></param>
        private void OpenWindow(Form Frm)
        {
            Frm.TopLevel = false;
            Frm.FormBorderStyle = FormBorderStyle.None;
            Frm.Parent = this.panel1;
            Frm.Dock = DockStyle.Fill;
            Frm.Show();
        }
        #endregion

        private void 检测界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseWindow("FrmTest") == false)
            {
                FrmTest objFrm = new FrmTest();
                OpenWindow(objFrm);
            }
        }

        private void 查看历史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Global.IsShow1 = false;
            // Global.IsShow2 = false;
            if (CloseWindow("FrmHistory") == false)
            {
                FrmHistory objFrm = new FrmHistory();
                OpenWindow(objFrm);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Global.SubWorking1||Global.SubWorking2)
            {
                MessageBox.Show("还有工件在测试请等待测试结束后退出！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("确定要退出测试系统", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result==DialogResult.Cancel)
            {
                return;
            }
            Global.Working1 = false;
            Global.Working2 = false;
            Global.IsShow1 = false;
            Global.IsShow2 = false;
            Application.Exit();
        }

        private void 导出文件设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "导出文件目标文件夹";
            if (dialog.ShowDialog()==DialogResult.OK)
            {
                CommonMethods.WriteConfig(dialog.SelectedPath);
            }
        }
        #region 拦截主窗口X按钮事件
        protected override void WndProc(ref Message msg)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg.Msg == WM_SYSCOMMAND && ((int)msg.WParam == SC_CLOSE))  // 点击winform右上关闭按钮 ，加入想要的逻辑处理
            { 
                if (Global.SubWorking1 || Global.SubWorking2)
                {
                    MessageBox.Show("还有工件在测试请等待测试结束后退出！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("确定要退出测试系统", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                Global.Working1 = false;
                Global.Working2 = false;
                Global.IsShow1 = false;
                Global.IsShow2 = false;
                Application.Exit();
            }
            base.WndProc(ref msg);
        }
        #endregion

    }

}
