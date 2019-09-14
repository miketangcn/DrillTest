namespace DrillTest
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnect1Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnect2Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.检测界面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看历史ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出文件设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoOutXlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnect1Status,
            this.lblConnect2Status});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lblConnect1Status
            // 
            this.lblConnect1Status.Name = "lblConnect1Status";
            resources.ApplyResources(this.lblConnect1Status, "lblConnect1Status");
            // 
            // lblConnect2Status
            // 
            this.lblConnect2Status.Name = "lblConnect2Status";
            resources.ApplyResources(this.lblConnect2Status, "lblConnect2Status");
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.功能ToolStripMenuItem,
            this.设置ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // 功能ToolStripMenuItem
            // 
            this.功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.检测界面ToolStripMenuItem,
            this.查看历史ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            resources.ApplyResources(this.功能ToolStripMenuItem, "功能ToolStripMenuItem");
            // 
            // 检测界面ToolStripMenuItem
            // 
            this.检测界面ToolStripMenuItem.Image = global::DrillTest.Properties.Resources.CHECKMRK;
            this.检测界面ToolStripMenuItem.Name = "检测界面ToolStripMenuItem";
            resources.ApplyResources(this.检测界面ToolStripMenuItem, "检测界面ToolStripMenuItem");
            this.检测界面ToolStripMenuItem.Click += new System.EventHandler(this.检测界面ToolStripMenuItem_Click);
            // 
            // 查看历史ToolStripMenuItem
            // 
            this.查看历史ToolStripMenuItem.Image = global::DrillTest.Properties.Resources.DataBase;
            this.查看历史ToolStripMenuItem.Name = "查看历史ToolStripMenuItem";
            resources.ApplyResources(this.查看历史ToolStripMenuItem, "查看历史ToolStripMenuItem");
            this.查看历史ToolStripMenuItem.Click += new System.EventHandler(this.查看历史ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Image = global::DrillTest.Properties.Resources.Close;
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            resources.ApplyResources(this.退出ToolStripMenuItem, "退出ToolStripMenuItem");
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导出文件设置ToolStripMenuItem,
            this.autoOutXlsToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            resources.ApplyResources(this.设置ToolStripMenuItem, "设置ToolStripMenuItem");
            // 
            // 导出文件设置ToolStripMenuItem
            // 
            this.导出文件设置ToolStripMenuItem.Image = global::DrillTest.Properties.Resources.保存;
            this.导出文件设置ToolStripMenuItem.Name = "导出文件设置ToolStripMenuItem";
            resources.ApplyResources(this.导出文件设置ToolStripMenuItem, "导出文件设置ToolStripMenuItem");
            this.导出文件设置ToolStripMenuItem.Click += new System.EventHandler(this.导出文件设置ToolStripMenuItem_Click);
            // 
            // autoOutXlsToolStripMenuItem
            // 
            this.autoOutXlsToolStripMenuItem.Checked = true;
            this.autoOutXlsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoOutXlsToolStripMenuItem.Name = "autoOutXlsToolStripMenuItem";
            resources.ApplyResources(this.autoOutXlsToolStripMenuItem, "autoOutXlsToolStripMenuItem");
            this.autoOutXlsToolStripMenuItem.Click += new System.EventHandler(this.AutoOutXlsToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            resources.ApplyResources(this.帮助ToolStripMenuItem, "帮助ToolStripMenuItem");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 检测界面ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看历史ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripStatusLabel lblConnect1Status;
        private System.Windows.Forms.ToolStripStatusLabel lblConnect2Status;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出文件设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoOutXlsToolStripMenuItem;
    }
}

