using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DrillTest.Model;
using DrillTest.Lib;

namespace DrillTest
{
    public partial class FrmTest : Form
    {
        private AutoResetEvent AutoResetEvent1 = new AutoResetEvent(true);
        private AutoResetEvent AutoResetEvent2 = new AutoResetEvent(true);
        public static FrmTest frmtest;
        public FrmTest()
        {
            InitializeComponent();
            frmtest = this;//指定本窗口便于其他类调用本窗体控件
            this.Load += FrmTest_Load;
            lblx1.DataBindings.Add("Text", Global.CurrentPoint1, "X");
            lbly1.DataBindings.Add("Text", Global.CurrentPoint1, "Y");
            lblx2.DataBindings.Add("Text", Global.CurrentPoint2, "X");
            lbly2.DataBindings.Add("Text", Global.CurrentPoint2, "Y");
            txtHole1.DataBindings.Add("Text", Global.CurrentHoleCount1, "Count");
            txtHole2.DataBindings.Add("Text", Global.CurrentHoleCount2, "Count");
        }
        private void FrmTest_Load(object sender, EventArgs e)
        {
            Series series1 = new Series("S1");
            Series series2 = new Series("S2");
            panel1.Width = this.Width / 2;
            panel2.Width = this.Width / 2;
            chart1.Height = this.Height - 160;
            chart2.Height = this.Height - 160;
            //this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            //this.chart1.ChartAreas[0].AxisY.Maximum = 12;
            this.chart1.ChartAreas[0].AxisX.Minimum = Global.min_x;
            this.chart1.ChartAreas[0].AxisX.Maximum = Global.max_x;
            this.chart1.ChartAreas[0].AxisX.Interval = 10;
            this.chart1.ChartAreas[0].AxisY.Interval = 1;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Black;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Black;
            this.chart1.Series.Add(series1);
            this.chart1.Series[0].ChartType = SeriesChartType.Spline;
            this.chart1.Series[0].IsVisibleInLegend = false;
            this.chart1.Series[0].Points.AddXY(-1, 0);
            this.chart1.Series[0].Color = Color.Red;
            this.chart1.Series[0].ToolTip = "#VAL{N3}Mpa;#VALX{N3}mm";
            lblToolTip1.Visible = false;
            this.chart2.ChartAreas[0].AxisX.Minimum = Global.min_x;
            this.chart2.ChartAreas[0].AxisX.Maximum = Global.max_x;
            this.chart2.ChartAreas[0].AxisX.Interval = 10;
            this.chart2.ChartAreas[0].AxisY.Interval = 1;
            this.chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Black;
            this.chart2.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            this.chart2.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            this.chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Black;
            this.chart2.Series.Add(series2);
            this.chart2.Series[0].ChartType = SeriesChartType.Spline;
            this.chart2.Series[0].IsVisibleInLegend = false;
            this.chart2.Series[0].Points.AddXY(-1, 0);
            this.chart2.Series[0].Color = Color.Red;
            this.chart2.Series[0].ToolTip = "#VAL{N3}Mpa;#VALX{N3}mm";
            lblToolTip2.Visible = false;
            Thread Thread1 = new Thread(ShowCurrent1);
            Thread1.IsBackground = true;
            Thread1.Start();
            Thread Thread2 = new Thread(ShowCurrent2);
            Thread2.IsBackground = true;
            Thread2.Start();
            btnReDo1.Enabled = Global.SubWorking1;
            btnReDo2.Enabled = Global.SubWorking2;
            if (Global.Working1)
            {
                txtHole1.Text = Global.HoleNumber1.ToString();
                txtSN1.Text = Global.WorkRecord1.Id;
                txtLayer1.Text = Global.WorkRecord1.Layer.ToString();
                ControlEnable(true, Global.Working1);
            }
            if (Global.Working2)
            {
                txtHole2.Text = Global.HoleNumber2.ToString();
                txtSN2.Text = Global.WorkRecord2.Id;
                txtLayer2.Text = Global.WorkRecord2.Layer.ToString();
                ControlEnable(false, Global.Working2);
            }

        }

        private void FrmTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.IsShow1 = false;
            Global.IsShow2 = false;
            Thread.Sleep(100);
            AutoResetEvent1.WaitOne();
            AutoResetEvent2.WaitOne();
        }

        private void FrmTest_Resize(object sender, EventArgs e)
        {
            panel1.Width = this.Width / 2;
            panel2.Width = this.Width / 2;
            chart1.Height = this.Height - 160;
            chart2.Height = this.Height - 160;

        }

        #region 设置重新测试按钮使能（在测试线程中调用）
        /// <summary>
        /// 修改按钮属性，此处主要用来在其他线程中来设置本控件属性
        /// </summary>
        public void SetRedo1Ena(bool Ena)// 在其他类的线程中访问（修改）窗体中控件属性的方法，见印象笔记
        {
            if (btnReDo1.InvokeRequired)//指示是否必须调用invoke方法
            {
                Invoke(new Action(() => btnReDo1.Enabled =Ena));
            } 
        }
        public void SetRedo2Ena(bool Ena)
        {
            if (btnReDo2.InvokeRequired)
            {
                Invoke(new Action(() => btnReDo2.Enabled = Ena));
            }
        }
        private void ControlEnable(bool Is1, bool Ena)
        {
            if (Is1)
            {

                btnStart1.Enabled = !Ena;
                btnStop1.Enabled = Ena;
                txtLayer1.Enabled = !Ena;
                txtSN1.Enabled = !Ena;
            }
            else
            {
                btnStop2.Enabled = Ena;
                btnStart2.Enabled = !Ena;
                txtLayer2.Enabled = !Ena;
                txtSN2.Enabled = !Ena;
            }
        }
        #endregion

        #region 曲线画面刷新
        private void ShowCurrent2()
        {
            AutoResetEvent2.Reset();
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            Global.IsShow2 = true;
            while (Global.IsShow2)
            {
                if (Global.SubWorking2)
                {
                    x.Clear();
                    y.Clear();
                    if (Global.lstPoint2.Count > 0)
                    {
                        for (int i = 0; i < Global.lstPoint2.Count; i++)
                        {
                            float px, py;
                            px = Global.con_factor_x * Global.lstPoint2[i].x;
                            py = Global.con_factor_y * Global.lstPoint2[i].y;
                            x.Add(px);
                            y.Add(py);
                        }
                    }
                }
                if (Global.IsShow2)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (x.Count > 0)
                        {
                            chart2.Series[0].Points.Clear();
                            chart2.Series[0].Points.DataBindXY(x, y);
                        }
                        lblInformation2.Visible = Global.SubWorking2;
                    }));
                }
                
                Thread.Sleep(200);
                if (!Global.IsShow2) break;
            }
            AutoResetEvent2.Set();
        }
        private void ShowCurrent1()
        {
            AutoResetEvent1.Reset();
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            Global.IsShow1 = true;
            while (Global.IsShow1)
            {

                if (Global.SubWorking1)
                {
                    x.Clear();
                    y.Clear();
                    if (Global.lstPoint1.Count > 0)
                    {
                        for (int i = 0; i < Global.lstPoint1.Count; i++)
                        {
                            float px, py;
                            px = Global.con_factor_x * Global.lstPoint1[i].x;
                            py = Global.con_factor_y * Global.lstPoint1[i].y;
                            x.Add(px);
                            y.Add(py);
                        }
                    }
                }
                if (Global.IsShow1)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (x.Count > 0)
                        {
                            chart1.Series[0].Points.Clear();
                            chart1.Series[0].Points.DataBindXY(x, y);
                        }
                        lblInformation1.Visible = Global.SubWorking1;
                    }));
                }
                else break;
                Thread.Sleep(200);
                if (!Global.IsShow1) break;
            }
           AutoResetEvent1.Set();
        }
        #endregion

        #region tooltip处理
        private void Chart1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var area = chart1.ChartAreas[0];
                double xValue = area.AxisX.PixelPositionToValue(e.X);
                double yValue = area.AxisY.PixelPositionToValue(e.Y);
                lblToolTip1.Text = string.Format("位移:{0:F2}mm 载荷:{1:F2}Mpa ", xValue, yValue);
                lblToolTip1.Location = new System.Drawing.Point(e.X, e.Y + chart1.Top);
                chart1.ChartAreas[0].CursorX.Position = xValue;
                chart1.ChartAreas[0].CursorY.Position = yValue;
            }
            catch (Exception)
            {
               // throw;
            }
 
        }

        private void Chart1_MouseEnter(object sender, EventArgs e)
        {
            lblToolTip1.Visible = true;
        }

        private void Chart1_MouseLeave(object sender, EventArgs e)
        {
            lblToolTip1.Visible = false;
        }

        private void Chart2_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var area = chart2.ChartAreas[0];
                double xValue = area.AxisX.PixelPositionToValue(e.X);
                double yValue = area.AxisY.PixelPositionToValue(e.Y);
                lblToolTip2.Text = string.Format("位移:{0:F2}mm 载荷:{1:F2}Mpa ", xValue, yValue);
                lblToolTip2.Location = new System.Drawing.Point(e.X, e.Y + chart2.Top);
                chart2.ChartAreas[0].CursorX.Position = xValue;
                chart2.ChartAreas[0].CursorY.Position = yValue;
            }
            catch (Exception)
            {

            }

        }

        private void Chart2_MouseLeave(object sender, EventArgs e)
        {
            lblToolTip2.Visible = false;
        }

        private void Chart2_MouseEnter(object sender, EventArgs e)
        {
            lblToolTip2.Visible = true;
        }
        #endregion

        #region 画面按钮处理
        private void BtnRedo1_Click(object sender, EventArgs e)
        {
            btnReDo1.Enabled = false;
            if (Global.SubWorking1)
            {
                MessageBox.Show("#1压机正在压制中请稍等停止", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Global.Working1)
            {
                MessageBox.Show("#1压机还未处在检测待机状态", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Global.HoleNumber1>1)
            {
                Global.HoleNumber1--;
            }
        }
        private void BtnReDo2_Click(object sender, EventArgs e)
        {

            btnReDo2.Enabled = false;
            if (Global.SubWorking2)
            {
                MessageBox.Show("#2压机正在压制中请稍等停止", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Global.Working2)
            {
                MessageBox.Show("#2压机还未处在检测待机状态", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Global.HoleNumber2 > 1)
            {
                Global.HoleNumber2--;
            }

        }
        private async void BtnStart1_Click(object sender, EventArgs e)
        {
            Global.HoleNumber1 = 0;
            string WorkId = txtSN1.Text.Trim();
            if (txtLayer1.Text.Trim()=="")
            {
                txtLayer1.Text = "1";
            }
            if (txtSN1.Text.Trim() == "")
            {
                WorkId = DateTime.Now.Date.ToString("yyyyMMdd") + DateTime.Now.TimeOfDay.ToString("hhmm");
                txtSN1.Text = WorkId;
            }
            //判断#2台是否有相同号工作
            if (Global.Working2 && WorkId== txtSN2.Text.Trim())
            {
                MessageBox.Show("测试工件号已在#2台测试，请变更工件号", "警告", MessageBoxButtons.OK);
                return;
            }
            Task task = new Task(() =>
            {
              string sql = "select HoleCount from Work where SerialNO= '{0}'";
              sql = string.Format(sql, WorkId);
              DataSet ds = SQLHelper.GetDataSet(sql);
              string sqlHole = "select HoleNumber from HoleTestRec where SerialNO= '{0}'";
              sqlHole = string.Format(sqlHole, WorkId);
              DataSet dsHole;
              Global.HoleNumber1 = 1;
              if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
              {
                  DialogResult dialogResult = MessageBox.Show("测试工件号已存在，继续测试还是变更工件号", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                  if (dialogResult == DialogResult.No)
                  {
                      Invoke(new Action(()=>
                      {
                          txtSN1.Focus();
                      }));                        
                      return;
                  }
                  else
                  {
                   //继续测试读取已经完成的洞号
                      dsHole = SQLHelper.GetDataSet(sqlHole);
                     if (dsHole != null && dsHole.Tables.Count != 0 && dsHole.Tables[0].Rows.Count != 0)
                     {
                       Global.HoleNumber1 = Convert.ToInt16(dsHole.Tables[0].Rows[dsHole.Tables[0].Rows.Count - 1]["HoleNumber"]);
                       Global.HoleNumber1++;
                     }
                  }
              }
              });
            task.Start();
            await task;
            Global.CurrentHoleCount1.Count= Global.HoleNumber1.ToString();
            Global.WorkRecord1.Id = WorkId;
            Global.WorkRecord1.Layer = Convert.ToInt16(txtLayer1.Text);
            Global.WorkRecord1.HoleCount = Global.HoleNumber1;
            Global.Working1 = true;
            ReadValue.Distance = 50;
            ReadValue.Pressure = 350;
            ReadValue.IsMax = false;
            Global.DateFileName1 = WorkId + ".xls";
            ControlEnable(true, Global.Working1);
        }

        private void BtnStop1_Click(object sender, EventArgs e)
        {
            if (Global.SubWorking1)
            {
                MessageBox.Show("#1压机正在压制中请稍等停止", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Global.Working1 = false;
            ControlEnable(true, Global.Working1);
            //btnReDo1.Enabled = true;
        }


        private void BtnStop2_Click(object sender, EventArgs e)
        {
            if (Global.SubWorking2)
            {
                MessageBox.Show("#2压机正在压制中请稍等停止", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Global.Working2 = false;
            ControlEnable(false, Global.Working2);
            //btnReDo2.Enabled = true;
        }
        private async void BtnStart2_Click(object sender, EventArgs e)
        {
            Global.HoleNumber2 = 0;
            string WorkId = txtSN2.Text.Trim();
            if (txtLayer2.Text.Trim() == "")
            {
                txtLayer2.Text = "1";
            }
            if (txtSN2.Text.Trim() == "")
            {
                WorkId = DateTime.Now.Date.ToString("yyyyMMdd") + DateTime.Now.TimeOfDay.ToString("hhmm");
                txtSN2.Text = WorkId;
            }
            //判断#2台是否有相同号工作
            if (Global.Working1 && WorkId == txtSN1.Text.Trim())
            {
                MessageBox.Show("测试工件号已在#1台测试，请变更工件号", "警告", MessageBoxButtons.OK);
                return;
            }
            Task task = new Task(() =>
            {
                string sql = "select HoleCount from Work where SerialNO= '{0}'";
                sql = string.Format(sql, WorkId);
                DataSet ds = SQLHelper.GetDataSet(sql);
                string sqlHole = "select HoleNumber from HoleTestRec where SerialNO= '{0}'";
                sqlHole = string.Format(sqlHole, WorkId);
                DataSet dsHole;
                Global.HoleNumber2 = 1;
                if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                {
                    DialogResult dialogResult = MessageBox.Show("测试工件号已存在，继续测试还是变更工件号", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                    {
                        Invoke(new Action(() =>
                        {
                            txtSN2.Focus();
                        }));
                        return;
                    }
                    else
                    {
                        //继续测试读取已经完成的洞号
                        dsHole = SQLHelper.GetDataSet(sqlHole);
                        if (dsHole != null && dsHole.Tables.Count != 0 && dsHole.Tables[0].Rows.Count != 0)
                        {
                            Global.HoleNumber2 = Convert.ToInt16(dsHole.Tables[0].Rows[dsHole.Tables[0].Rows.Count - 1]["HoleNumber"]);
                            Global.HoleNumber2++;
                        }
                    }
                }
            });
            task.Start();
            await task;
            Global.CurrentHoleCount2.Count = Global.HoleNumber1.ToString();
            Global.WorkRecord2.Id = WorkId;
            Global.WorkRecord2.Layer = Convert.ToInt16(txtLayer2.Text);
            Global.WorkRecord2.HoleCount = Global.HoleNumber2;
            Global.Working2 = true;
            Global.DateFileName2 = WorkId + ".xls";
            ControlEnable(false, Global.Working2);
        }
        #endregion
    }
}