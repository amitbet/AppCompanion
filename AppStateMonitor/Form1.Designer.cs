namespace AppStateMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.attachDebugMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.takeADumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sampleAppStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logExceptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getProcessStacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabExceptionLog = new System.Windows.Forms.TabPage();
            this.txtExceptionLog = new System.Windows.Forms.TextBox();
            this.tabPerformance = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabDebugConsole = new System.Windows.Forms.TabPage();
            this.txtDebugConsole = new System.Windows.Forms.TextBox();
            this.tabCallstack = new System.Windows.Forms.TabPage();
            this.txtCallstack = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrProcListener = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabExceptionLog.SuspendLayout();
            this.tabPerformance.SuspendLayout();
            this.tabDebugConsole.SuspendLayout();
            this.tabCallstack.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attachDebugMonitorToolStripMenuItem,
            this.takeADumpToolStripMenuItem,
            this.sampleAppStateToolStripMenuItem,
            this.logExceptionsToolStripMenuItem,
            this.getProcessStacksToolStripMenuItem,
            this.toolStripSeparator1,
            this.showWindowToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(281, 178);
            // 
            // attachDebugMonitorToolStripMenuItem
            // 
            this.attachDebugMonitorToolStripMenuItem.Name = "attachDebugMonitorToolStripMenuItem";
            this.attachDebugMonitorToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.attachDebugMonitorToolStripMenuItem.Text = "Attach debug console monitor";
            this.attachDebugMonitorToolStripMenuItem.Click += new System.EventHandler(this.attachDebugMonitorToolStripMenuItem_Click);
            // 
            // takeADumpToolStripMenuItem
            // 
            this.takeADumpToolStripMenuItem.Name = "takeADumpToolStripMenuItem";
            this.takeADumpToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.takeADumpToolStripMenuItem.Text = "Take a Memory dump";
            this.takeADumpToolStripMenuItem.Click += new System.EventHandler(this.takeADumpToolStripMenuItem_Click);
            // 
            // sampleAppStateToolStripMenuItem
            // 
            this.sampleAppStateToolStripMenuItem.Name = "sampleAppStateToolStripMenuItem";
            this.sampleAppStateToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.sampleAppStateToolStripMenuItem.Text = "Take a profile snapshot";
            this.sampleAppStateToolStripMenuItem.Click += new System.EventHandler(this.sampleAppStateToolStripMenuItem_Click);
            // 
            // logExceptionsToolStripMenuItem
            // 
            this.logExceptionsToolStripMenuItem.Name = "logExceptionsToolStripMenuItem";
            this.logExceptionsToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.logExceptionsToolStripMenuItem.Text = "Listen to exceptions";
            this.logExceptionsToolStripMenuItem.Click += new System.EventHandler(this.listenToProcessToolStripMenuItem_Click);
            // 
            // getProcessStacksToolStripMenuItem
            // 
            this.getProcessStacksToolStripMenuItem.Name = "getProcessStacksToolStripMenuItem";
            this.getProcessStacksToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.getProcessStacksToolStripMenuItem.Text = "Get CallStacks";
            this.getProcessStacksToolStripMenuItem.Click += new System.EventHandler(this.getProcessStacksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(277, 6);
            // 
            // showWindowToolStripMenuItem
            // 
            this.showWindowToolStripMenuItem.Name = "showWindowToolStripMenuItem";
            this.showWindowToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.showWindowToolStripMenuItem.Text = "Show Window";
            this.showWindowToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Location = new System.Drawing.Point(2, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(967, 322);
            this.panel1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabExceptionLog);
            this.tabControl1.Controls.Add(this.tabPerformance);
            this.tabControl1.Controls.Add(this.tabDebugConsole);
            this.tabControl1.Controls.Add(this.tabCallstack);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(967, 322);
            this.tabControl1.TabIndex = 2;
            // 
            // tabExceptionLog
            // 
            this.tabExceptionLog.Controls.Add(this.txtExceptionLog);
            this.tabExceptionLog.Location = new System.Drawing.Point(4, 25);
            this.tabExceptionLog.Name = "tabExceptionLog";
            this.tabExceptionLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabExceptionLog.Size = new System.Drawing.Size(959, 293);
            this.tabExceptionLog.TabIndex = 0;
            this.tabExceptionLog.Text = "Exception log";
            this.tabExceptionLog.UseVisualStyleBackColor = true;
            // 
            // txtExceptionLog
            // 
            this.txtExceptionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExceptionLog.Location = new System.Drawing.Point(3, 3);
            this.txtExceptionLog.Multiline = true;
            this.txtExceptionLog.Name = "txtExceptionLog";
            this.txtExceptionLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExceptionLog.Size = new System.Drawing.Size(953, 287);
            this.txtExceptionLog.TabIndex = 0;
            // 
            // tabPerformance
            // 
            this.tabPerformance.Controls.Add(this.treeView1);
            this.tabPerformance.Location = new System.Drawing.Point(4, 25);
            this.tabPerformance.Name = "tabPerformance";
            this.tabPerformance.Padding = new System.Windows.Forms.Padding(3);
            this.tabPerformance.Size = new System.Drawing.Size(959, 293);
            this.tabPerformance.TabIndex = 1;
            this.tabPerformance.Text = "Performance Tree";
            this.tabPerformance.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(953, 287);
            this.treeView1.TabIndex = 0;
            // 
            // tabDebugConsole
            // 
            this.tabDebugConsole.Controls.Add(this.txtDebugConsole);
            this.tabDebugConsole.Location = new System.Drawing.Point(4, 25);
            this.tabDebugConsole.Name = "tabDebugConsole";
            this.tabDebugConsole.Size = new System.Drawing.Size(959, 293);
            this.tabDebugConsole.TabIndex = 2;
            this.tabDebugConsole.Text = "Debug console";
            this.tabDebugConsole.UseVisualStyleBackColor = true;
            // 
            // txtDebugConsole
            // 
            this.txtDebugConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebugConsole.Location = new System.Drawing.Point(0, 0);
            this.txtDebugConsole.Multiline = true;
            this.txtDebugConsole.Name = "txtDebugConsole";
            this.txtDebugConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDebugConsole.Size = new System.Drawing.Size(959, 293);
            this.txtDebugConsole.TabIndex = 1;
            // 
            // tabCallstack
            // 
            this.tabCallstack.Controls.Add(this.txtCallstack);
            this.tabCallstack.Location = new System.Drawing.Point(4, 25);
            this.tabCallstack.Name = "tabCallstack";
            this.tabCallstack.Size = new System.Drawing.Size(959, 293);
            this.tabCallstack.TabIndex = 3;
            this.tabCallstack.Text = "Callstack";
            this.tabCallstack.UseVisualStyleBackColor = true;
            // 
            // txtCallstack
            // 
            this.txtCallstack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCallstack.Location = new System.Drawing.Point(0, 0);
            this.txtCallstack.Multiline = true;
            this.txtCallstack.Name = "txtCallstack";
            this.txtCallstack.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCallstack.Size = new System.Drawing.Size(959, 293);
            this.txtCallstack.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(2, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(967, 44);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // tmrProcListener
            // 
            this.tmrProcListener.Enabled = true;
            this.tmrProcListener.Interval = 1000;
            this.tmrProcListener.Tick += new System.EventHandler(this.tmrProcListener_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 373);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabExceptionLog.ResumeLayout(false);
            this.tabExceptionLog.PerformLayout();
            this.tabPerformance.ResumeLayout(false);
            this.tabDebugConsole.ResumeLayout(false);
            this.tabDebugConsole.PerformLayout();
            this.tabCallstack.ResumeLayout(false);
            this.tabCallstack.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem logExceptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem takeADumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sampleAppStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getProcessStacksToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrProcListener;
        private System.Windows.Forms.TabControl tabControl1;
 
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox txtExceptionLog;
        private System.Windows.Forms.ToolStripMenuItem showWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attachDebugMonitorToolStripMenuItem;
        private System.Windows.Forms.TabPage tabExceptionLog;
        private System.Windows.Forms.TabPage tabPerformance;
        private System.Windows.Forms.TabPage tabDebugConsole;
        private System.Windows.Forms.TabPage tabCallstack;
        private System.Windows.Forms.TextBox txtDebugConsole;
        private System.Windows.Forms.TextBox txtCallstack;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

