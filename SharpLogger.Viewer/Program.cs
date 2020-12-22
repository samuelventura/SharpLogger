using System;
using System.Drawing;
using System.Windows.Forms;
using SharpTabs;

namespace SharpLogger.Viewer
{
    static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var path = args.Length > 0 ? args[0] : null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var factory = new LogFactory(path);
            TabsTools.SetupCatcher(factory.Name);
            var form = new MainForm(factory);
            form.Size = new Size(1200, 600);
            Application.Run(form);
        }
    }
}
