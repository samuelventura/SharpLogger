using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace SharpLogger.Tryout
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetupCatcher("Tryout");
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

        public static void SetupCatcher(string name)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => { HandleException(name, e.Exception); };
            AppDomain.CurrentDomain.UnhandledException += (s, e) => { HandleException(name, e.ExceptionObject as Exception); };
        }

        public static string Dump(string folder, Exception ex)
        {
            var exceptions = Path.Combine(folder, "Exceptions");
            Directory.CreateDirectory(exceptions);
            var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var file = string.Format("exception-{0}.txt", ts);
            var path = Path.Combine(exceptions, file);
            File.WriteAllText(path, ex.ToString());
            return path;
        }

        public static void HandleException(string name, Exception ex)
        {
            var folder = DefaultFolder(name);
            var path = Dump(folder, ex);
            Process.Start(path);
            MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1); //Application.Exit calls MainForm.OnClose
        }

        public static string DefaultFolder(string name)
        {
            var entry = Assembly.GetEntryAssembly().Location;
            return Path.GetDirectoryName(entry);
        }
    }
}
