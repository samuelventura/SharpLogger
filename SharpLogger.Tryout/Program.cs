using System;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var dlg = new MainForm();
            dlg.ShowDialog();
            switch (dlg.Tryout)
            {
                case 1:
                    LogDebug.AddDefaultFile();
                    LogDebug.Enable(typeof(LogPanel).Name);
                    LogDebug.Enable(typeof(LogModel).Name);
                    Application.Run(new PanelTryoutForm());
                    break;
                case 2:
                    Application.Run(new ControlTryoutForm());
                    break;
            }
        }
    }
}
