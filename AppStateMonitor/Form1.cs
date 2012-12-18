using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProcessExaminator;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Threading;

namespace AppStateMonitor
{
    public partial class Form1 : Form
    {
        string m_strProcName = "uft";
        string m_strDumpFilename = DebuggerUtils.GetOutputDirectory() + "\\dump" + DebuggerUtils.GetTimeString(true) + ".dmp";
        bool m_processIconSet = false;

        public Form1()
        {
            InitializeComponent();
            string procName = Config.GetStringValue(Config.TargetProcessName);
            if (procName != null)
                m_strProcName = procName;
        }

        private void listenToProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcListener == null)
                return;

            if (!logExceptionsToolStripMenuItem.Checked)
            {
                ProcListener.StartListening();
                ProcListener.LogExceptions = true;
                label1.Text = "Last file saved: " + ProcListener.LastLogFileName;
                notifyIcon1.ShowBalloonTip(4, "saved", ProcListener.LastLogFileName, ToolTipIcon.Info);
                sampleAppStateToolStripMenuItem.Enabled = false;
                getProcessStacksToolStripMenuItem.Enabled = false;

                tabControl1.SelectedTab = tabControl1.TabPages["tabExceptionLog"];
            }
            else
            {
                ProcListener.LogExceptions = false;
                ProcListener.StopListening();

                sampleAppStateToolStripMenuItem.Enabled = true;
                getProcessStacksToolStripMenuItem.Enabled = true;

            }
            logExceptionsToolStripMenuItem.Checked = !logExceptionsToolStripMenuItem.Checked;
        }

        private void takeADumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //just take a dump..
            notifyIcon1.ShowBalloonTip(4, "dumping", "dumping process memory...", ToolTipIcon.Info);
            m_strDumpFilename = DebuggerUtils.GetOutputDirectory() + "\\dump" + DateTime.Now.ToString("ddMMyyyy_HHmmssfff") + ".dmp";
            DumpWriter.WriteDump(DebuggerUtils.GetFirstProcessWithName(m_strProcName).Id, m_strDumpFilename, DumpOptions.WithFullMemory);
            label1.Text = "Last file saved: " + m_strDumpFilename;
            notifyIcon1.ShowBalloonTip(4, "saved", m_strDumpFilename, ToolTipIcon.Info);
        }

        ProcessListener m_procListener = null;

        int ProcessId { get; set; }

        public ProcessListener ProcListener
        {
            get
            {
                Process p = null;
                try
                {
                    p = Process.GetProcessById(ProcessId);
                }
                catch (ArgumentException ex)
                {
                    m_procListener = null;
                }

                if (p == null)
                    m_procListener = null;


                if (m_procListener == null)
                {
                    Task tskAttachTask = null;
                    Process proc = DebuggerUtils.GetFirstProcessWithName(m_strProcName);
                    if (proc == null)
                    {
                        m_procListener = null;
                        return m_procListener;
                    }

                    ProcessId = proc.Id;
                    notifyIcon1.Text = "Loading application assemblies (a few seconds)";
                    if (proc != null)
                    {
                        m_procListener = new ProcessListener(proc.Id);
                        tskAttachTask = m_procListener.InitialAttach();
                    }
                    try
                    {
                        if (tskAttachTask != null)
                            tskAttachTask.Wait();
                        
                        notifyIcon1.Text = "App Sidekick Ready.";
                    }
                    catch (Exception ex)
                    {
                        string message = ex.GetBaseException().Message;
                        if (ex.GetBaseException().Message.Contains("incompatible platforms"))
                            message = message.Replace("(Exception from HRESULT: 0x80131C30)", " (Recompile AppComapnion in as x86/x64 to match process architecture)");

                        notifyIcon1.ShowBalloonTip(10, "Attach Error", message, ToolTipIcon.Error);
                        notifyIcon1.Text = "Attach Error";
                        DisableAllCommands();
                        notifyIcon1.BalloonTipClosed += new EventHandler(notifyIcon1_BalloonTipClosed);
                        notifyIcon1.BalloonTipClicked += new EventHandler(notifyIcon1_BalloonTipClosed);
                        notifyIcon1.MouseClick -= notifyIcon1_MouseClick;
                        notifyIcon1.MouseClick +=new MouseEventHandler(notifyIcon1_ClosingMouseClick);
                    }
                }
                return m_procListener;
            }
        }

        private void notifyIcon1_ClosingMouseClick(object sender, MouseEventArgs e)
        {
            CloseApp();
        }
        void notifyIcon1_BalloonTipClosed(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void CloseApp()
        {
            notifyIcon1.Dispose();
            Environment.Exit(1);
        }

        private void DisableAllCommands()
        {
            foreach (ToolStripItem item in contextMenuStrip1.Items)
                if (item.Text != "Exit")
                    item.Enabled = false;
        }
        //private void dumpOnUncaughtExceptionToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    dumpOnUncaughtExceptionToolStripMenuItem.Checked = !dumpOnUncaughtExceptionToolStripMenuItem.Checked;
        //    DumpOnUncaughtException = dumpOnUncaughtExceptionToolStripMenuItem.Checked;

        //    ProcListener.WriteDumpOnUncaughtExceptions = m_blnDumpOnUncaughtException;
        //    ProcListener.StartListening();
        //}

        private void BuildTreeView(TreeNode parent, StackTreeNode stackTree)
        {
            foreach (StackTreeNode snode in stackTree.Children)
            {
                TreeNode newNode = parent.Nodes.Add(snode.ToString());

                foreach (StackTreeNode schild in snode.Children)
                    BuildTreeView(newNode, schild);
            }
        }

        private void sampleAppStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<int, StackTreeNode> sample = null;
            if (ProcListener == null)
                return;

            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    notifyIcon1.ShowBalloonTip(4, "Sampling", "application process is being examined.", ToolTipIcon.Info);
                    sample = ProcListener.GetPerformanceSample();

                    string strSampleFilename = DebuggerUtils.GetOutputDirectory() + "\\sample" + DebuggerUtils.GetTimeString(true) + ".sample";
                    ProcListener.SavePerformanceSampleToBin(sample, strSampleFilename);


                    if (!this.IsHandleCreated) return;
                    treeView1.Invoke((ThreadStart)
                    delegate
                    {
                        label1.Text = "Last file saved: " + strSampleFilename;
                        notifyIcon1.ShowBalloonTip(4, "saved", strSampleFilename, ToolTipIcon.Info);

                        treeView1.Nodes.Clear();
                        //TreeView tv = new TreeView();
                        foreach (var root in sample)
                        {
                            TreeNode threadRoot = treeView1.Nodes.Add(root.Key.ToString());
                            //TreeNode stackRoot = threadRoot.Nodes.Add(root.Value.ToString());
                            BuildTreeView(threadRoot, root.Value);
                        }
                        tabControl1.SelectedTab = tabControl1.TabPages["tabPerformance"];
                    });
                }
                catch (Exception ex)
                {
                    DebuggerUtils.HandleException(ex);
                }

            });
            //richTextBox1.Hide();
            //panel1.Controls.Add(tv);
            //tv.Dock = DockStyle.Fill;
            //tv.Show();


            //take a multi - sample, create a tree, write tree to file.
            //ProcessSampler s = new ProcessSampler(false);
        }

        private void getProcessStacksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcListener == null)
                return;

            List<ProcessSample> samples = null;



            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    samples = ProcListener.GetCallStacks(false, true);

                    if (!this.IsHandleCreated) return;

                    txtExceptionLog.Invoke((ThreadStart)delegate
                    {
                        if (samples != null && samples.Count > 0)
                        {
                            ProcessSample sample = samples[0];
                            txtCallstack.Text = sample.ToString();
                            txtCallstack.SelectionStart = txtCallstack.Text.Length;
                            txtCallstack.ScrollToCaret();
                            txtCallstack.Refresh();
                            notifyIcon1.ShowBalloonTip(4, "Done.", "callstack captured", ToolTipIcon.Info);
                            tabControl1.SelectedTab = tabControl1.TabPages["tabCallstack"];
                        }
                    });
                }
                catch (Exception ex)
                {
                    DebuggerUtils.HandleException(ex);
                }
            });
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeWindowVisibility();
        }

        private void ChangeWindowVisibility()
        {
            if (showWindowToolStripMenuItem.Text == "Show Window")
            {
                this.Show();
                WindowState = FormWindowState.Normal;
                this.Focus();
                showWindowToolStripMenuItem.Text = "Hide Window";
            }
            else
            {
                this.Hide();
                showWindowToolStripMenuItem.Text = "Show Window";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void attachDebugMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!attachDebugMonitorToolStripMenuItem.Checked)
            {
                DebugMonitor.Start();

                DebugMonitor.OnOutputDebugString += new OnOutputDebugStringHandler(DebugMonitor_OnOutputDebugString);
            }
            else
            {
                DebugMonitor.OnOutputDebugString -= new OnOutputDebugStringHandler(DebugMonitor_OnOutputDebugString);
                DebugMonitor.Stop();
            }
            ///				Console.WriteLine("Press 'Enter' to exit.");
            ///				Console.ReadLine();

            attachDebugMonitorToolStripMenuItem.Checked = !attachDebugMonitorToolStripMenuItem.Checked;

            tabControl1.SelectedTab = tabControl1.TabPages["tabDebugConsole"];
        }

        void DebugMonitor_OnOutputDebugString(int pid, string text)
        {
            if (!this.IsHandleCreated) return;

            this.Invoke((ThreadStart)delegate
            {
                txtDebugConsole.Text += pid + ": " + text;
                txtDebugConsole.SelectionStart = txtDebugConsole.Text.Length;
                txtDebugConsole.ScrollToCaret();
                txtDebugConsole.Refresh();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CloseApplication();
        }

        private void CloseApplication()
        {
            if (m_procListener != null)
            {
                if (ProcListener.IsListening)
                    ProcListener.StopListening();

                m_procListener.CleanUp();
                notifyIcon1.Visible = false;
            }
            Environment.Exit(0);
        }

        void tmrProcListener_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!m_processIconSet)
                {
                    Process p = DebuggerUtils.GetFirstProcessWithName(m_strProcName);
                    if (p != null)
                    {
                        string procExeFilename = p.MainModule.FileName;
                        Icon ico = Icon.ExtractAssociatedIcon(procExeFilename);

                        if (ico != null)
                        {
                            this.Icon = ico;
                            notifyIcon1.Icon = ico;
                            m_processIconSet = true;
                        }
                    }
                }
            }
            catch
            {
                //no process icon to set, no problem
            }


            if (ProcListener != null && txtExceptionLog.Text.Length != ProcListener.ProcessLog.Length && ProcListener.IsListening)
            {
                txtExceptionLog.Text = ProcListener.ProcessLog;
                txtExceptionLog.SelectionStart = txtExceptionLog.Text.Length;
                txtExceptionLog.ScrollToCaret();
                txtExceptionLog.Refresh();
            }

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                ChangeWindowVisibility();
        }
    }
}
