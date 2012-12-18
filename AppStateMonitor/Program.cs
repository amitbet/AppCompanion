using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ProcessExaminator;

namespace AppStateMonitor
{
    static class Program
    {
     
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
          
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 f = new Form1();
            f.WindowState = FormWindowState.Minimized;
            
            f.Show();
            f.Hide();

            Application.Run();
        }
    }
}
