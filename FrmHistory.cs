using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DrillTest.Lib;
using DrillTest.Model;

namespace DrillTest
{
    public partial class FrmHistory : Form
    {
        private string SelectedWorkId;
        private DataSet ds = new DataSet();
        private string LastWorkId;
        public FrmHistory()
        {
            InitializeComponent();
            this.Load += FrmHistory_Load;
            chart1.MouseEnter += Chart1_MouseEnter;
        }
        #region tooltip处理
        private void Chart1_MouseEnter(object sender, EventArgs e)
        {
            lblToolTip1.Visible = true;
        }
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

            }

        }

        private void Chart1_MouseLeave(object sender, EventArgs e)
        {
            lblToolTip1.Visible = false;
        }
        #endregion
        private async void FrmHistory_Load(object sender, EventArgs e)
        {
            Series series1 = new Series("S1");
            int year=DateTime.Now.Year;
            this.splitContainer1.SplitterDistance = 365;
            panel1.Width = splitContainer1.Panel2.Width / 2 - 1;
            panel2.Width = panel1.Width;
            dateTimePicker1.Text = year.ToString() + "/1/1";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView1.Columns[0].DataPropertyName = "SerialNO";
            dataGridView1.Columns[1].DataPropertyName = "Layer";
            dataGridView1.Columns[2].DataPropertyName = "HoleCount";
            dataGridView1.Columns[3].DataPropertyName = "LastTime";
            dataGridView2.Width = splitContainer1.Panel1.Width;
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.Columns[0].DataPropertyName = "SerialNO";
            dataGridView2.Columns[1].DataPropertyName = "HoleNumber";
            dataGridView2.Columns[2].DataPropertyName = "MaxPressure";
            dataGridView2.Columns[3].DataPropertyName = "TestTime";
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.Width = splitContainer1.Panel1.Width;
            rdoDate.Select();
            this.chart1.ChartAreas[0].AxisX.Minimum = 200;
            this.chart1.ChartAreas[0].AxisX.Maximum = 350;
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
            DateTime t1 = Convert.ToDateTime(this.dateTimePicker1.Text);
            DateTime t2 = Convert.ToDateTime(this.dateTimePicker2.Text).AddDays(1);
            Task task = new Task(() =>
             {
                 QueryWorkByDate(t1,t2);
             });
            task.Start();
            await task;

        }
        private  void QueryWorkByDate(DateTime t1, DateTime t2)
        {
            string sql="";

                  sql = "Select * from Work where LastTime Between '{0}' and '{1}' order by LastTime desc";
                  sql = string.Format(sql, t1, t2);
                  Invoke(new Action(() =>
                  {
                      dataGridView1.DataSource = SQLHelper.GetDataSet(sql).Tables[0];
                  }));
                  Global.LastQueryMode = false;
                  Global.LastQueryTFrom = t1;
                  Global.LastQueryTTo = t2;
        }
        private void QueryWorkBySN( string Id)
        {
                if (Id == null || Id == "")
                {
                    MessageBox.Show("工件号不能为空", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Id = "%" + Id + "%";
                string sql = "Select * from Work where SerialNO Like '{0}' order by LastTime desc";
                sql = string.Format(sql, Id);
                dataGridView1.DataSource = SQLHelper.GetDataSet(sql).Tables[0];
            Global.LastQueryMode = true;
            Global.LastQueryId = Id;
        }
        private void QueryHoleRec(string Id)
        {
            string sql = "Select * from HoleTestRec where SerialNo = '{0}'  order by TestTime desc";
            sql = string.Format(sql, Id);
            ds= SQLHelper.GetDataSet(sql);
            dataGridView2.DataSource = ds.Tables[0];
            if (dataGridView2.Rows.Count>0)
            {
               chart2.Series.Clear();
               ShowAllCurve();   
            }    
        }
        private async void ShowAllCurve()
        {
            Task task = new Task(() =>
              {
                  Invoke(new Action(() =>
                  {
                      int n = ds.Tables[0].Rows.Count;
                      this.chart2.ChartAreas[0].AxisX.Minimum = 200;
                      this.chart2.ChartAreas[0].AxisX.Maximum = 350;
                      this.chart2.ChartAreas[0].AxisX.Interval = 10;
                      this.chart2.ChartAreas[0].AxisY.Interval = 1;
                      this.chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Black;
                      this.chart2.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
                      this.chart2.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
                      this.chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Black;
                      for (int i = 0; i < n; i++)
                      {
                          string Name = "S" + i.ToString();
                          Series series = new Series(Name);
                          this.chart2.Series.Add(series);
                          this.chart2.Series[i].ChartType = SeriesChartType.Spline;
                          this.chart2.Series[i].IsVisibleInLegend = false;
                          this.chart2.Series[i].Points.AddXY(-1, 0);
                          this.chart2.Series[i].Color = Color.Red;
                      }

                      for (int k = 0; k < n; k++)
                      {
                          List<float> x = new List<float>();
                          List<float> y = new List<float>();
                          var lstSingleCurve = new List<Model.Point>();
                          string strings = ds.Tables[0].Rows[k]["Data"].ToString();
                          lstSingleCurve = CommonMethods.TFromBinary(strings).ConvertAll(s => (Model.Point)s);
                          x.Clear();
                          y.Clear();
                          if (lstSingleCurve.Count > 0)
                          {
                              for (int l = 0; l < lstSingleCurve.Count; l++)
                              {
                                  float px, py;
                                  px = Global.con_factor_x * lstSingleCurve[l].x;
                                  py = Global.con_factor_y * lstSingleCurve[l].y;
                                  x.Add(px);
                                  y.Add(py);
                              }

                              chart2.Series[k].Points.Clear();
                              chart2.Series[k].Points.DataBindXY(x, y);

                          }
                      }
                  }));
                 
              });
            task.Start();
            await task;
        }
        private async void ShowSingleCurve(int index)
        {
          Task task=new Task(() =>
          {
            List<float> x = new List<float>();
            List<float> y = new List<float>();
            var lstSingleCurve = new List<Model.Point>();
            string strings = ds.Tables[0].Rows[index]["Data"].ToString();
            lstSingleCurve = CommonMethods.TFromBinary(strings).ConvertAll(s=>(Model.Point)s);
            x.Clear();
            y.Clear();
            if (lstSingleCurve.Count > 0)
            {
                for (int i = 0; i < lstSingleCurve.Count; i++)
                {
                    float px, py;
                    px = Global.con_factor_x * lstSingleCurve[i].x;
                    py = Global.con_factor_y * lstSingleCurve[i].y;
                    x.Add(px);
                    y.Add(py);
                }
                Invoke(new Action(() =>
                {
                    chart1.Series[0].Points.Clear();
                    chart1.Series[0].Points.DataBindXY(x, y);
                }));
            }
        });
            task.Start();
            await task;
    }
        private void FrmHistory_Resize(object sender, EventArgs e)
        {
            panel1.Width = splitContainer1.Panel2.Width / 2 - 1;
            panel2.Width = panel1.Width;
            dataGridView1.Top = 200;
            dataGridView1.Left = 0;
            dataGridView1.Height = (this.Height - 200) / 2 - 2;
            dataGridView2.Top = dataGridView1.Bottom + 2;
            dataGridView2.Height = dataGridView1.Height;
            dataGridView1.Width = dataGridView2.Width;
        }
/// <summary>
/// 选择变化
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count>0)
            {
               
               int index = dataGridView1.SelectedCells[0].RowIndex;
                // dataGridView1.CurrentCell = dataGridView1[0, index];
                SelectedWorkId = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (SelectedWorkId==LastWorkId) return;

               LastWorkId = SelectedWorkId;
               QueryHoleRec(SelectedWorkId);
            }
           
        }

        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells.Count > 0)
            {
                int index = dataGridView2.SelectedCells[0].RowIndex;
                ShowSingleCurve(index);
            }
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            DateTime t1 = Convert.ToDateTime(this.dateTimePicker1.Text);
            DateTime t2 = Convert.ToDateTime(this.dateTimePicker2.Text).AddDays(1);
            if (rdoDate.Checked)
            {
                QueryWorkByDate(t1,t2);
            }
            else if (rdoSN.Checked)
            {
                string Id = txtSN.Text.Trim();
                QueryWorkBySN(Id);
            }
        }

        private void BtnDeleteHole_Click(object sender, EventArgs e)
        {

            if (dataGridView2.SelectedCells.Count>0)
            {
                string Id = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                Int16 HoleId=Convert.ToInt16(dataGridView2.CurrentRow.Cells[1].Value);
                Int16 Count = Convert.ToInt16(dataGridView2.RowCount - 1);
                string sql1 = "Delete from HoleTestRec where SerialNo = '{0}' and HoleNumber= {1} ";
                sql1 = string.Format(sql1, Id,HoleId);
                string sql2 = "Update Work Set HoleCount= {0}  where SerialNo = '{1}' ";
                sql2 = string.Format(sql2, Count, Id);
                List<string> sql = new List<string>
                {
                    sql1,
                    sql2
                };
                DialogResult result = MessageBox.Show("确定要删除工件"+Id+"洞号"+HoleId.ToString()+"的记录？", "警告",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    if (SQLHelper.UpdateByTran(sql))
                    {
                        DateTime t1 = Convert.ToDateTime(this.dateTimePicker1.Text);
                        DateTime t2 = Convert.ToDateTime(this.dateTimePicker2.Text).AddDays(1);
                        MessageBox.Show("删除成功", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        QueryWorkByDate(t1,t2);
                        QueryHoleRec(SelectedWorkId);
                    }
                    else MessageBox.Show("删除失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
          
        }

        private void BtnDeleteWork_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count>0)
            {
                Global.LastDataRow = dataGridView1.CurrentRow.Index;
                string Id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string sql1 = "Delete from HoleTestRec where SerialNo = '{0}' ";
                string sql2 = "Delete from Work where SerialNo = '{0}' ";
                sql1 = string.Format(sql1, Id);
                sql2 = string.Format(sql2, Id);
                List<string> sql = new List<string>
                {
                    sql1,
                    sql2
                };
                DialogResult result = MessageBox.Show("确定要删除工件"+Id+"的记录？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result==DialogResult.OK)
                {
                    if (SQLHelper.UpdateByTran(sql))
                    {
                        MessageBox.Show("删除成功","信息",MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (Global.LastQueryMode)
                        {
                            QueryWorkBySN(Global.LastQueryId);
                        }
                        else
                        {
                            QueryWorkByDate(Global.LastQueryTFrom, Global.LastQueryTTo);
                        }
                        if ( Global.LastDataRow< dataGridView1.Rows.Count)
                        {
                            dataGridView1.Rows[Global.LastDataRow].Selected = true;
                        }
                        else if (dataGridView1.Rows.Count > 0)
                        {
                            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                        }

                    }
                    else MessageBox.Show("删除失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            string Id;
            int n = ds.Tables[0].Rows.Count;
            if (dataGridView1.SelectedRows.Count>0 && n>0)
            {
                Id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "File (*.xls)|*.xls";
                dialog.Title = "导出工件" + Id + "记录";
                dialog.InitialDirectory = Global.OutputPath; ;
                dialog.FileName = Id;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (CommonMethods.GetFilePath(dialog.FileName))
                    {
                        for (int i = 0; i < n; i++)
                        {
                            int k = i + 1;
                            var lstSingleCurve = new List<Model.Point>();
                            string strings = ds.Tables[0].Rows[i]["Data"].ToString();
                            lstSingleCurve = CommonMethods.TFromBinary(strings).ConvertAll(s => (Model.Point)s);
                            NopiExcelHelper<Model.Point>.AddExcel(lstSingleCurve, dialog.FileName, "hole" + k.ToString());
                        }
                        
                    }

                }
                
            }

        }
    }
}
