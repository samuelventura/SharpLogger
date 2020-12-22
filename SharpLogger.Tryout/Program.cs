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
            var tryout = 0;
            switch (tryout)
            {
                case 0:
                    LogDebug.AddDefaultFile();
                    LogDebug.Enable(typeof(LogPanel).Name);
                    LogDebug.Enable(typeof(LogModel).Name);
                    Application.Run(new PanelTryoutForm());
                    break;
                case 1:
                    Application.Run(new ControlTryoutForm());
                    break;
            }
        }
    }
}
